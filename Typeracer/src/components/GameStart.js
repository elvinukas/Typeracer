import React from 'react';

function GameStart({ onStart }) {
    const startGame = () => {
        const audio = new Audio('/sounds/click.mp3');
        audio.play();
        audio.onended = () => {
            onStart();  // Making the transition to the Type component
        };
    };

    return (
        <div className="fullscreen-container">
            <div className="text-center">
                <h1 className="display-1" style={{ fontFamily: 'Courier Prime, monospace' }}>
                    RAŠYMO LENKTYNĖS
                </h1>
                <button className="btn btn-primary btn-lg mt-3" onClick={startGame}>
                    Pradėti žaidimą
                </button>
            </div>
        </div>
    );
}

export default GameStart;
