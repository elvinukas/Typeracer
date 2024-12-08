import React, {useState} from 'react';
import '../../wwwroot/css/GameStart.css';
import CustomAlert from './CustomAlert';
import {useGame} from "./GameContext";

function GameStart({ onStart }) {
    const [username, setUsername] = useState('');
    const {gamemode, setGamemode} = useGame();
    const [showAlert, setShowAlert] = useState(false);
    const startGame = async () => {
        if (!username) {
            setShowAlert(true);
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
        <div className="game-start-body">
            <div className="title-container">
                <p className="game-start-title">
                    RAŠYMO LENKTYNĖS
                </p>
            </div>
            <div className="input-container">
                <input className="input-box"
                       type="text"
                       placeholder="Įveskite savo vartotojo vardą"
                       value={username}
                       onChange={(e) => setUsername(e.target.value)}
                       size={29}
                />
            </div>
            <div className="dropdown-container">
                <label htmlFor="gamemode">Pasirinkite žaidimo tipą:</label>
                <select
                    id="gamemode"
                    className="dropdown"
                    value={gamemode}
                    onChange={(e) => setGamemode(e.target.value)}
                >
                    <option value="0">Standartinis</option>
                    <option value="1">Trumpas</option>
                    <option value="2">Sunkus</option>
                </select>
            </div>
            <div className="button-container">
                <button
                    className="start-game-button"
                    onClick={startGame}
                >
                    Pradėti žaidimą
                </button>
            </div>
            {showAlert && <CustomAlert message="Vartotojo vardas būtinas!" borderColor="red"
                                       onClose={() => setShowAlert(false)}/>}
        </div>
    );
}

export default GameStart;