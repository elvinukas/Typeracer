import React, {useState} from 'react';

function GameStart({ onStart }) {
    const [username, setUsername] = useState('');
    const startGame = async () => {
        if (!username) {
            alert("Vartotojo vardas būtinas!");
            return;
        }
        
        try {
            const response = await fetch('/api/leaderboard/save-username', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(username)
            });
            
            if (!response.ok) {
                throw new Error('Failed to save username');
            }

            const audio = new Audio('/sounds/click.mp3');
            audio.play();
            audio.onended = () => {
                onStart();  // Making the transition to the Type component
            };
        } catch (error) {
            console.error('Error saving username:', error);
        }
    };

    return (
        <div className="fullscreen-container">
            <div className="text-center">
                <h1 className="display-1" style={{fontFamily: 'Courier Prime, monospace'}}>
                    RAŠYMO LENKTYNĖS
                </h1>
                <input
                    type="text"
                    placeholder="Įveskite savo vartotojo vardą"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                />
                <button className="btn btn-primary btn-lg mt-3" onClick={startGame}>
                    Pradėti žaidimą
                </button>
            </div>
        </div>
    );
}

export default GameStart;
