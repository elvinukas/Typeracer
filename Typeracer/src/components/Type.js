import React, { useState, useEffect, useRef } from 'react';
import '../../wwwroot/css/Type.css';

function Type() {
    const [typingText, setTypingText] = useState('');
    const [currentIndex, setCurrentIndex] = useState(0);
    const charRefs = useRef([]);

    useEffect(() => {
        const fetchParagraphText = async () => {
            let response = await fetch('/Home/GetParagraphText/');
            let jsonResponse = await response.json();
            setTypingText(jsonResponse.text);
        };

        fetchParagraphText();
    }, []);

    useEffect(() => {
        const handleKeyDown = (event) => {
            let inputCharacter = event.key;

            if (currentIndex < typingText.length && inputCharacter === typingText[currentIndex]) {
                setCurrentIndex((prevIndex) => prevIndex + 1);
            }
        };

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
                            style={{ color: index < currentIndex ? 'green' : 'grey' }}
                        >
                            {char}
                        </span>
                    ))}
                </p>
            </div>

            <div className="button-container">
                <button className="restart-button" onClick={() => window.location.reload()}>
                    Pradėti iš naujo
                </button>
                <button className="next-text-button">
                    Kitas tekstas
                </button>
            </div>
        </div>
    );
}

export default Type;