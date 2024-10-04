import React, { useState, useEffect, useRef } from 'react';
import { Howl } from 'howler';
import '../../wwwroot/css/Type.css';
import wrongSound from '../../wwwroot/sounds/incorrect.mp3';
import GameData from './GameData';

function Type() {
    const [typingText, setTypingText] = useState('');
    const [initialText, setInitialText] = useState('');
    const [currentIndex, setCurrentIndex] = useState(0);
    const [incorrectChars, setIncorrectChars] = useState({});
    const [firstErrorIndex, setFirstErrorIndex] = useState(null);
    const [consecutiveRedCount, setConsecutiveRedCount] = useState(0);
    const [startTime, setStartTime] = useState(null);
    const [elapsedTime, setElapsedTime] = useState(0);
    const [isComplete, setIsComplete] = useState(false);
    const [showGameData, setShowGameData] = useState(false);
    
    // used for checking when was the last keypress recorded - text cursor blinker
    const [lastKeyPressTime, setLastKeyPressTime] = useState(Date.now());
    const [isBlinking, setIsBlinking] = useState(true);

    // Statistics data object for sending to the server
    const [statisticsData, setStatisticsData] = useState({
        CompletionTime: 0,
        LocalStartTime: null,
        LocalFinishTime: null,
        TotalAmountOfWords: 0,
        TotalAmountOfCharacters: 0,
        TypedAmountOfWords: 0,
        TypedAmountOfCharacters: 0,
        NumberOfWrongfulCharacters: 0,
        TypingData: []
    });
    
    const charRefs = useRef([]);
    const wrongSoundRef = useRef(null);
    const intervalRef = useRef(null);
    const blinkTimeoutRef = useRef(null);
    const errorWordInfoRef = useRef(null);
    
    // Used for storing word information
    const wordsInfoRef = useRef([]);

    const fetchParagraphText = async () => {
        let response = await fetch('/Home/GetParagraphText/');
        let jsonResponse = await response.json();
        setTypingText(jsonResponse.text);
        setInitialText(jsonResponse.text);

        // Calculating the total amount of words and characters
        const totalWords = jsonResponse.text.trim().split(/\s+/).length;
        setStatisticsData(prevData => ({
            ...prevData,
            TotalAmountOfWords: totalWords,
            TotalAmountOfCharacters: jsonResponse.text.length
        }));

        // Creating wordsInfoRef
        if (jsonResponse.text) {
            let tempWordsInfo = [];
            let index = 0;
            jsonResponse.text.split(' ').forEach((word) => {
                tempWordsInfo.push({
                    word: word,
                    startIndex: index,
                    endIndex: index + word.length - 1,
                    mistakes: 0,
                    startTime: null,
                    endTime: null,
                });
                index += word.length + 1; // +1 for the space
            });
            wordsInfoRef.current = tempWordsInfo;
        }
    };

    useEffect(() => {
        fetchParagraphText();

        // Initializing Howler sound
        wrongSoundRef.current = new Howl({
            src: [wrongSound],
            preload: true,
        });

        return () => {
            clearInterval(intervalRef.current);
            clearTimeout(blinkTimeoutRef.current);
        };
    }, []);

    const handleKeyDown = async (event) => {
        const inputCharacter = event.key;
        const isCharacterKey = inputCharacter.length === 1 || inputCharacter === ' ';

        setLastKeyPressTime(Date.now());
        setIsBlinking(false);
        clearTimeout(blinkTimeoutRef.current);

        // Text cursor blinking logic
        blinkTimeoutRef.current = setTimeout(() => {
            setIsBlinking(true);
        }, 530);

        // starts the timer when the first character is typed
        if (currentIndex === 0 && !startTime) {
            const start = Date.now();
            setStartTime(start);
            setStatisticsData(prevData => ({
                ...prevData,
                LocalStartTime: new Date(start)
            }));
            clearInterval(intervalRef.current);
            intervalRef.current = setInterval(() => {
                setElapsedTime(Date.now() - start);
            }, 1000);
        }

        let newCurrentIndex = currentIndex; // Initializing newCurrentIndex

        const adjustedIndex = currentIndex;
        const wordIndex = wordsInfoRef.current.findIndex(
            wordInfo => adjustedIndex >= wordInfo.startIndex && adjustedIndex <= wordInfo.endIndex
        );
        
        
        let wordInfo = null;
        if (firstErrorIndex !== null && errorWordInfoRef.current) {
            // Using the fixed wordInfo
            wordInfo = errorWordInfoRef.current;
        } else if (wordIndex !== -1) {
            wordInfo = wordsInfoRef.current[wordIndex];
            // Recording the start time of the word
            if (!wordInfo.startTime) {
                wordInfo.startTime = Date.now();
            }
        }

        if (inputCharacter === 'Backspace') {
            if (currentIndex > 0) {
                newCurrentIndex = currentIndex - 1;
                setCurrentIndex(newCurrentIndex);

                // Removing the incorrect character if it exists
                setIncorrectChars((prevIncorrectChars) => {
                    const newIncorrectChars = { ...prevIncorrectChars };
                    delete newIncorrectChars[newCurrentIndex];
                    return newIncorrectChars;
                });

                // Resetting the first error index if needed
                if (firstErrorIndex !== null && newCurrentIndex <= firstErrorIndex) {
                    setFirstErrorIndex(null);
                    errorWordInfoRef.current = null; // Resetting the fixed word
                }

                // Resetting the consecutive red count
                setConsecutiveRedCount((prevCount) => Math.max(prevCount - 1, 0));

                // Updating statistics data
                setStatisticsData(prevData => ({
                    ...prevData,
                    TypedAmountOfCharacters: Math.max(prevData.TypedAmountOfCharacters - 1, 0)
                }));
            }
        } else if (consecutiveRedCount < 20) { // allow typing only if less than 20 consecutive red characters
            if (currentIndex < typingText.length && inputCharacter === typingText[currentIndex]) { // checking if the input character is correct
                setIncorrectChars((prevIncorrectChars) => {
                    const newIncorrectChars = { ...prevIncorrectChars };
                    delete newIncorrectChars[currentIndex];
                    return newIncorrectChars;
                });

                newCurrentIndex = currentIndex + 1;
                setCurrentIndex(newCurrentIndex);

                if (firstErrorIndex !== null && currentIndex >= firstErrorIndex) {
                    setConsecutiveRedCount((prevCount) => prevCount + 1);
                    if (wrongSoundRef.current) {
                        wrongSoundRef.current.play();
                    }
                } else {
                    setConsecutiveRedCount(0);
                }

                // Updating statistics data
                setStatisticsData(prevData => ({
                    ...prevData,
                    TypedAmountOfCharacters: prevData.TypedAmountOfCharacters + 1
                }));

                // Checking if the word is completed
                if (wordInfo && currentIndex === wordInfo.endIndex) {
                    if (!wordInfo.endTime) {
                        wordInfo.endTime = Date.now();
                        // Incrementing TypedAmountOfWords
                        setStatisticsData(prevData => ({
                            ...prevData,
                            TypedAmountOfWords: prevData.TypedAmountOfWords + 1
                        }));
                    }
                }

            } else if (isCharacterKey && inputCharacter !== typingText[currentIndex]) {  // Incorrect character
                // Recalculating wordInfo if null
                if (!wordInfo) {
                    const adjustedIndex = currentIndex > 0 ? currentIndex - 1 : 0;
                    const wordIndex = wordsInfoRef.current.findIndex(
                        wordInfo => adjustedIndex >= wordInfo.startIndex && adjustedIndex <= wordInfo.endIndex
                    );

                    if (wordIndex !== -1) {
                        wordInfo = wordsInfoRef.current[wordIndex];
                        // Recording the start time of the word
                        if (!wordInfo.startTime) {
                            wordInfo.startTime = Date.now();
                        }
                    } else {
                        console.log('wordInfo is still null after recalculation at currentIndex:', currentIndex);
                    }
                }

                setIncorrectChars((prevIncorrectChars) => ({
                    ...prevIncorrectChars,
                    [currentIndex]: inputCharacter,
                }));

                newCurrentIndex = currentIndex + 1;
                setCurrentIndex(newCurrentIndex);

                if (firstErrorIndex === null) {
                    setFirstErrorIndex(currentIndex);
                    errorWordInfoRef.current = wordInfo; // Fixing the word where the first error occurred
                }

                setConsecutiveRedCount((prevCount) => prevCount + 1);
                if (wrongSoundRef.current) {
                    wrongSoundRef.current.play();
                }

                // Incrementing mistakes for the fixed word
                const currentErrorWordInfo = errorWordInfoRef.current;
                if (currentErrorWordInfo) {
                    currentErrorWordInfo.mistakes += 1;
                    console.log(`Incremented mistakes for word "${currentErrorWordInfo.word}" to ${currentErrorWordInfo.mistakes}`);
                } else {
                    console.log('Unable to increment mistakes because wordInfo is null');
                }

                // Updating TypedAmountOfCharacters
                setStatisticsData(prevData => ({
                    ...prevData,
                    TypedAmountOfCharacters: prevData.TypedAmountOfCharacters + 1
                }));
            }
        } else{
            event.preventDefault();
        }

        // Using newCurrentIndex for completion check
        if (!isComplete && newCurrentIndex >= typingText.length) {  // stops the timer when the text is finished and the last character is typed
            clearInterval(intervalRef.current);
            const finishTime = Date.now();

            // Collecting updated values
            const newLocalFinishTime = new Date(finishTime);
            const newCompletionTime = finishTime - startTime;

            // Preparing the typingData array
            const typingData = wordsInfoRef.current.map(wordInfo => ({
                Word: wordInfo.word,
                BeginningTimestampWord: wordInfo.startTime ? formatDateTime(new Date(wordInfo.startTime)) : null,
                EndingTimestampWord: wordInfo.endTime ? formatDateTime(new Date(wordInfo.endTime)) : null,
                AmountOfMistakesInWord: wordInfo.mistakes
            }));

            // Calculating the total number of wrongful characters
            const totalMistakes = typingData.reduce((acc, word) => acc + word.AmountOfMistakesInWord, 0);

            // Recalculating TypedAmountOfWords
            const newTypedAmountOfWords = typingData.filter(td => td.EndingTimestampWord !== null).length;

            // Collecting all updated statistics data
            const updatedStatisticsData = {
                ...statisticsData,
                LocalFinishTime: newLocalFinishTime,
                CompletionTime: newCompletionTime,
                TypedAmountOfWords: newTypedAmountOfWords,
                TypedAmountOfCharacters: currentIndex, // Using currentIndex as the total typed characters
                NumberOfWrongfulCharacters: totalMistakes,
                TypingData: typingData
            };

            // Updating the statisticsData state
            setStatisticsData(updatedStatisticsData);

            // Assembling the data to send
            const dataToSend = {
                CompletionTime: updatedStatisticsData.CompletionTime,
                LocalStartTime: updatedStatisticsData.LocalStartTime ? formatDateTime(updatedStatisticsData.LocalStartTime) : null,
                LocalFinishTime: updatedStatisticsData.LocalFinishTime ? formatDateTime(updatedStatisticsData.LocalFinishTime) : null,
                TotalAmountOfWords: updatedStatisticsData.TotalAmountOfWords,
                TotalAmountOfCharacters: updatedStatisticsData.TotalAmountOfCharacters,
                TypedAmountOfWords: updatedStatisticsData.TypedAmountOfWords,
                TypedAmountOfCharacters: updatedStatisticsData.TypedAmountOfCharacters,
                NumberOfWrongfulCharacters: updatedStatisticsData.NumberOfWrongfulCharacters,
                TypingData: updatedStatisticsData.TypingData
            };
            
            // Sending the data
            await sendStatisticsData(dataToSend);
            
            // Marking as completed to prevent duplicate requests
            setIsComplete(true);
        }
    };

    const formatDateTime = (date) => {
        return date.toISOString().replace('Z', '');
    };


    const sendStatisticsData = async (dataToSend) => {
        // Logging the data being sent
        console.log('Data being sent:', JSON.stringify(dataToSend, null, 2));

        try {
            const response = await fetch('/api/statistics/save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(dataToSend)
            });
            if (!response.ok) {
                const errorResponse = await response.json();
                console.error('Error response:', errorResponse);
                throw new Error('Network response was not ok');
            }
        } catch (error) {
            console.error('Error sending statistics data:', error);
        }
    };

    const resetStatisticsData = () => {
        setStatisticsData({
            CompletionTime: 0,
            LocalStartTime: null,
            LocalFinishTime: null,
            TotalAmountOfWords: statisticsData.TotalAmountOfWords,
            TotalAmountOfCharacters: statisticsData.TotalAmountOfCharacters,
            TypedAmountOfWords: 0,
            TypedAmountOfCharacters: 0,
            NumberOfWrongfulCharacters: 0,
            TypingData: []
        });
        wordsInfoRef.current = [];
    };

    useEffect(() => {
        document.addEventListener('keydown', handleKeyDown);

        return () => {
            document.removeEventListener('keydown', handleKeyDown);
        };
    }, [typingText, currentIndex]);

    useEffect(() => {
        const scrollAhead = 10;
        const scrollIndex = Math.min(currentIndex + scrollAhead, typingText.length - 1);
        const scrollElement = charRefs.current[scrollIndex];

        if (scrollElement) {
            scrollElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }, [currentIndex, typingText]);

    useEffect(() => { // fixes the problem when text resets or new text is fetched after spacebar press
        const preventSpacebarDefault = (event) => {
            if (event.key === ' ') {
                event.preventDefault();
            }
        };

        const buttons = document.querySelectorAll('.restart-button, .next-text-button');
        buttons.forEach(button => button.addEventListener('keydown', preventSpacebarDefault));

        return () => {
            buttons.forEach(button => button.removeEventListener('keydown', preventSpacebarDefault));
        };
    }, []);

    const formatTime = (time) => {
        const seconds = Math.floor((time / 1000) % 60);
        const minutes = Math.floor((time / (1000 * 60)) % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    const resetChronometer = () => {
        clearInterval(intervalRef.current);
        setStartTime(null);
        setElapsedTime(0);
    };

    useEffect(() => {
        if (isComplete) {
            fetch('/api/graph/generate', { // calls the API endpoint to generate the graph
                method: 'POST'
            })
                .then(response => response.json())
                .then(data => {
                    console.log(data.message);
                    setShowGameData(true);
                })
                .catch(error => console.error('Error generating graph:', error));
        }
    }, [isComplete]);

    if (showGameData) { // loads GameData.js page
        return <GameData />;
    }

    return (
        <div className="type-page-body">
            <div className="type-page-title">
                <p>Galima pradėti rašyti</p>
            </div>

            <div className="chronometer">
                <p>{formatTime(elapsedTime)}</p>
            </div>

            <div className="typing-container">
                <p className="typing-text">
                    {typingText.split('').map((char, index) => (
                        <span
                            key={index}
                            ref={(el) => (charRefs.current[index] = el)}
                            style={{
                                color: index < currentIndex
                                    ? firstErrorIndex !== null && index >= firstErrorIndex
                                        ? 'red'
                                        : 'green'
                                    : 'grey',
                                borderLeft: index === currentIndex ? '2px solid silver' : 'none',
                                animation: index === currentIndex && isBlinking ? 'blink 1s step-end infinite' : 'none'
                            }}
                        >
                            {incorrectChars[index] || char}
                        </span>
                    ))}
                </p>
            </div>

            <div className="button-container">
                <button className="restart-button" onClick={() => {
                    resetChronometer();
                    resetStatisticsData();
                    setTypingText(initialText);
                    setCurrentIndex(0);
                    setIncorrectChars({});
                    setFirstErrorIndex(null);
                    setConsecutiveRedCount(0);
                }}>
                    Pradėti iš naujo
                </button>
                <button
                    className="next-text-button"
                    onClick={async () => {
                        resetChronometer();
                        let response = await fetch('/Home/GetParagraphText/');
                        let jsonResponse = await response.json();
                        setTypingText(jsonResponse.text);
                        setInitialText(jsonResponse.text);

                        // Calculating the total amount of words and characters
                        const totalWords = jsonResponse.text.trim().split(/\s+/).length;

                        setStatisticsData({
                            CompletionTime: 0,
                            LocalStartTime: null,
                            LocalFinishTime: null,
                            TotalAmountOfWords: totalWords,
                            TotalAmountOfCharacters: jsonResponse.text.length,
                            TypedAmountOfWords: 0,
                            TypedAmountOfCharacters: 0,
                            NumberOfWrongfulCharacters: 0,
                            TypingData: []
                        });

                        // Creating wordsInfoRef
                        if (jsonResponse.text) {
                            let tempWordsInfo = [];
                            let index = 0;
                            jsonResponse.text.split(' ').forEach((word) => {
                                tempWordsInfo.push({
                                    word: word,
                                    startIndex: index,
                                    endIndex: index + word.length - 1,
                                    mistakes: 0,
                                    startTime: null,
                                    endTime: null,
                                });
                                index += word.length + 1;
                            });
                            wordsInfoRef.current = tempWordsInfo;
                        }

                        setCurrentIndex(0);
                        setIncorrectChars({});
                        setFirstErrorIndex(null);
                        setConsecutiveRedCount(0);
                    }}
                >
                    Kitas tekstas
                </button>
            </div>
        </div>
    );
}

export default Type;