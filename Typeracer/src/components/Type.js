import React, { useState, useEffect, useRef } from 'react';
import { Howl } from 'howler';
import '../../wwwroot/css/Type.css';
import wrongSound from '../../wwwroot/sounds/incorrect.mp3';

function Type() {
    const [typingText, setTypingText] = useState('');
    const [initialText, setInitialText] = useState('');
    const [currentIndex, setCurrentIndex] = useState(0);
    const [incorrectChars, setIncorrectChars] = useState({});
    const charRefs = useRef([]);
    const wrongSoundRef = useRef(null);

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
    }, []);

    const handleKeyDown = (event) => {
        const inputCharacter = event.key;
        const isCharacterKey = inputCharacter.length === 1;

        if (inputCharacter === 'Backspace') { // deleting characters
            if (currentIndex > 0) {
                setCurrentIndex((prevIndex) => prevIndex - 1);
                setIncorrectChars((prevIncorrectChars) => {
                    const newIncorrectChars = { ...prevIncorrectChars };
                    delete newIncorrectChars[currentIndex - 1];
                    return newIncorrectChars;
                });
            }
        } else if (currentIndex < typingText.length && inputCharacter === typingText[currentIndex]) { // checking if the input character is correct
            setIncorrectChars((prevIncorrectChars) => {
                const newIncorrectChars = { ...prevIncorrectChars };
                delete newIncorrectChars[currentIndex];
                return newIncorrectChars;
            });
            setCurrentIndex((prevIndex) => prevIndex + 1);
        } else if (isCharacterKey) { // checking if the input character is incorrect
            setIncorrectChars((prevIncorrectChars) => ({
                ...prevIncorrectChars,
                [currentIndex]: inputCharacter,
            }));
            setCurrentIndex((prevIndex) => prevIndex + 1);
            if (wrongSoundRef.current) {
                wrongSoundRef.current.play();
            }
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

    return (
        <div className="type-page-body">
            <div className="type-page-title">
                <p>Galima pradėti rašyti</p>
            </div>

            <div className="typing-container">
                <p className="typing-text">
                    {typingText.split('').map((char, index) => (
                        <span
                            key={index}
                            ref={(el) => (charRefs.current[index] = el)}
                            style={{
                                color: index < currentIndex 
                                    ? incorrectChars[index]
                                        ? 'red' 
                                        : 'green'
                                    :'grey' 
                            }}
                        >
                            {incorrectChars[index] || char}
                        </span>
                    ))}
                </p>
            </div>

            <div className="button-container">
                <button className="restart-button" onClick={() => {
                    setTypingText(initialText);
                    setCurrentIndex(0);
                    setIncorrectChars({});
                }}>
                    Pradėti iš naujo
                </button>
                <button
                    className="next-text-button"
                    onClick={async () => {
                        // Fetching new paragraph text from the server
                        let response = await fetch('/Home/GetParagraphText/');
                        let jsonResponse = await response.json();
                        setTypingText(jsonResponse.text);  // Setting the new text
                        setInitialText(jsonResponse.text);  // Updating the initial text
                        setCurrentIndex(0);  // Reseting the current index to the beginning
                        setIncorrectChars({});  // Clearing incorrect characters
                    }}
                >
                    Kitas tekstas
                </button>
            </div>
        </div>
    );
}

export default Type;