import React, {useContext, useState} from 'react';
import '../../wwwroot/css/GameStart.css';
import CustomAlert from './CustomAlert';
import { UsernameContext } from '../UsernameContext';

function GameStart({ onStart }) {
    const [showAlert, setShowAlert] = useState(false);
    const { setUsername } = useContext(UsernameContext);
    const [inputUsername, setInputUsername] = useState('');
    const startGame = async () => {
        if (!inputUsername) {
            setShowAlert(true);
            return;
        }

        setUsername(inputUsername);

        const audio = new Audio('/sounds/click.mp3');
        audio.play();
        audio.onended = () => {
            onStart();  // Making the transition to the Type component
        };
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
                    value={inputUsername}
                    onChange={(e) => setInputUsername(e.target.value)}
                       size={29}
                />
            </div>
            <div className="button-container">
                <button
                    className="start-game-button"
                    onClick={startGame}
                >
                    Pradėti žaidimą
                </button>
            </div>
            {showAlert && <CustomAlert message="Vartotojo vardas būtinas!" borderColor="red" onClose={() => setShowAlert(false)} />}
        </div>
    );
}

export default GameStart;