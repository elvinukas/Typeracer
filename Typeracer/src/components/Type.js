import React, { useState, useEffect } from 'react';
import '../../wwwroot/css/Type.css';

function Type() {
    const [typingText, setTypingText] = useState('');  // Atsisiųstas tekstas
    const [currentIndex, setCurrentIndex] = useState(0);  // Dabartinis simbolių indeksas

    useEffect(() => {
        const fetchParagraphText = async () => {
            let response = await fetch('/Home/GetParagraphText/');
            let jsonResponse = await response.json();
            setTypingText(jsonResponse.text);
        };

        fetchParagraphText();
    }, []);

    // Klavišų paspaudimų valdymas
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

    const typedText = typingText.substring(0, currentIndex);  // Teisingai įvestas tekstas
    const remainingText = typingText.substring(currentIndex);  // Likęs tekstas

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
