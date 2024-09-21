import React, { useState, useEffect } from 'react';
import '../../wwwroot/css/Type.css';

function Type() {
    const [typingText, setTypingText] = useState('');  // Text to type
    const [currentIndex, setCurrentIndex] = useState(0);  // Current index of the text

    useEffect(() => {
        const fetchParagraphText = async () => {
            let response = await fetch('/Home/GetParagraphText/');
            let jsonResponse = await response.json();
            setTypingText(jsonResponse.text);
        };

        fetchParagraphText();
    }, []);

    // Event listener for keydown
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

    const typedText = typingText.substring(0, currentIndex);  // Input text
    const remainingText = typingText.substring(currentIndex);  // Remaining text

    return (
        <div className="type-page-body">
            <div className="type-page-title">
                <p>Galima pradėti rašyti</p>
            </div>

            <div className="typing-container">
                <p className="typing-text">
                    <span style={{ color: 'green' }}>{typedText}</span>
                    {remainingText}
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
