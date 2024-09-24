import React, { useState, useEffect, useRef } from 'react';
import { Howl } from 'howler';
import '../../wwwroot/css/Type.css';
import wrongSound from '../../wwwroot/sounds/incorrect.mp3';

function Type() {
    const [typingText, setTypingText] = useState('');
    const [initialText, setInitialText] = useState('');
    const [currentIndex, setCurrentIndex] = useState(0);
    const [incorrectChars, setIncorrectChars] = useState({});
    const [firstErrorIndex, setFirstErrorIndex] = useState(null);
    const [consecutiveRedCount, setConsecutiveRedCount] = useState(0);
    const [startTime, setStartTime] = useState(null);
    const [elapsedTime, setElapsedTime] = useState(0);
    
    // used for checking when was the last keypress recorded - text cursor blinker
    const [lastKeyPressTime, setLastKeyPressTime] = useState(Date.now());
    const [isBlinking, setIsBlinking] = useState(false);
    
    const charRefs = useRef([]);
    const wrongSoundRef = useRef(null);
    const intervalRef = useRef(null);
    const blinkTimeoutRef = useRef(null);

    const fetchParagraphText = async () => {
        let response = await fetch('/Home/GetParagraphText/');
        let jsonResponse = await response.json();
        setTypingText(jsonResponse.text);
        setInitialText(jsonResponse.text);
    };
    
    useEffect(() => {
        fetchParagraphText();

        // Initializing Howler sound
        wrongSoundRef.current = new Howl({
            src: [wrongSound],
            preload: true,
        });

        return () => {
            // clear out the React variables
            clearInterval(intervalRef.current);
            clearTimeout(blinkTimeoutRef.current);
        };
    }, []);

    const handleKeyDown = (event) => {
        const inputCharacter = event.key;
        const isCharacterKey = inputCharacter.length === 1;
        
        setLastKeyPressTime(Date.now());
        setIsBlinking(false);
        clearTimeout(blinkTimeoutRef.current);
        
        // making the text cursor blink every second
        blinkTimeoutRef.current = setTimeout( () => {
            setIsBlinking(true);
        }, 1000);
        
        
        
        if (currentIndex === 0 && !startTime) { // starts the timer when the first character is typed
            const start = Date.now();
            setStartTime(start);
            clearInterval(intervalRef.current); // Clear any existing interval
            intervalRef.current = setInterval(() => {
                setElapsedTime(Date.now() - start);
            }, 1000);
        }

        if (inputCharacter === 'Backspace') { // deleting characters
            if (currentIndex > 0) {
                setCurrentIndex((prevIndex) => prevIndex - 1);
                setIncorrectChars((prevIncorrectChars) => {
                    const newIncorrectChars = { ...prevIncorrectChars };
                    delete newIncorrectChars[currentIndex - 1];
                    return newIncorrectChars;
                });
                if (firstErrorIndex !== null && currentIndex - 1 === firstErrorIndex) { // fixes the problem when incorrect letters would become green when deleting them
                    setFirstErrorIndex(null);
                }
                setConsecutiveRedCount((prevCount) => Math.max(prevCount - 1, 0));
            }
        } else if (consecutiveRedCount < 20) { // allow typing only if less than 20 consecutive red characters
            if (currentIndex < typingText.length && inputCharacter === typingText[currentIndex]) { // checking if the input character is correct
                setIncorrectChars((prevIncorrectChars) => {
                    const newIncorrectChars = { ...prevIncorrectChars };
                    delete newIncorrectChars[currentIndex];
                    return newIncorrectChars;
                });
                setCurrentIndex((prevIndex) => prevIndex + 1);
                if (firstErrorIndex !== null && currentIndex >= firstErrorIndex) { // plays the sound for correct (but red) letters because there was an error before
                    setConsecutiveRedCount((prevCount) => prevCount + 1);
                    if (wrongSoundRef.current) {
                        wrongSoundRef.current.play();
                    }
                } else {
                    setConsecutiveRedCount(0); // Reset the red count on correct character
                }
            } else if (isCharacterKey) { // checking if the input character is incorrect
                setIncorrectChars((prevIncorrectChars) => ({
                    ...prevIncorrectChars,
                    [currentIndex]: inputCharacter,
                }));
                setCurrentIndex((prevIndex) => prevIndex + 1);
                if (firstErrorIndex === null) { // sets the first error index
                    setFirstErrorIndex(currentIndex);
                }
                setConsecutiveRedCount((prevCount) => prevCount + 1);
                if (wrongSoundRef.current) {
                    wrongSoundRef.current.play();
                }
            }
        }
        
        if (currentIndex + 1 === typingText.length - 1 && inputCharacter === typingText[currentIndex]) { // stops the timer when the text is finished and the last character is typed
            clearInterval(intervalRef.current);
        }
    };

    useEffect(() => {
        document.addEventListener('keydown', handleKeyDown);

        return () => {
            document.removeEventListener('keydown', handleKeyDown);
        };
    }, [typingText, currentIndex]);

    useEffect(() => {
        const scrollAhead = 10; // This is the value that determines how many characters ahead to scroll
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

    const formatTime = (time) => { // formats the time in minutes and seconds
        const seconds = Math.floor((time / 1000) % 60);
        const minutes = Math.floor((time / (1000 * 60)) % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    const resetChronometer = () => {
        clearInterval(intervalRef.current);
        setStartTime(null);
        setElapsedTime(0);
    };

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
                                // adding text cursor animation to the border of a selected character
                                borderLeft: index === currentIndex ? '1px solid black' : 'none',
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
                        let response = await fetch('/Home/GetParagraphText/'); // Fetching new paragraph text from the server
                        let jsonResponse = await response.json();
                        setTypingText(jsonResponse.text);  // Setting the new text
                        setInitialText(jsonResponse.text);  // Updating the initial text
                        setCurrentIndex(0);  // Reseting the current index to the beginning
                        setIncorrectChars({});  // Clearing incorrect characters
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