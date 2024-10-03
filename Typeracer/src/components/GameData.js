import React, { useEffect, useState } from "react";
import '../../wwwroot/css/GameData.css';
function GameData() {
    const [gameData, setGameData] = useState(null);

    useEffect(() => {
        fetch('/statistics/game-data.json')
            .then(response => response.json())
            .then(data => setGameData(data))
            .catch(error => console.error('Error fetching game data:', error));
    }, []);

    if (!gameData) {
        return <div>...</div>; //loading screen
    }
    
    const completionTimeInSeconds = (gameData.CompletionTime / 1000).toFixed(2);
    const startTime = new Date(gameData.LocalStartTime);
    const formattedStartTime = startTime.toLocaleTimeString('en-GB', { hour12: false });
    const finishTime = new Date(gameData.LocalFinishTime);
    const formattedFinishTime = finishTime.toLocaleTimeString('en-GB', { hour12: false });
    
    return (
        <div className="game-data-body">
            <div className="game-data-title">
                <p>Žaidimo duomenys</p>
            </div>

            <div className="game-data-container">
                <div className="wpm-acc-graph">
                    <div className="wpm-acc">
                        <div className="wpm">
                            <div className="top-text">
                                <p className="paragraph">ŽPM</p>
                            </div>
                            <div className="top-number">
                                {gameData.WordsPerMinute.toFixed()}
                            </div>
                        </div>
                        <div className="acc">
                            <div className="top-text">
                                <p className="paragraph">TIKS.</p>
                            </div>
                            <div className="top-number">
                                {gameData.Accuracy.toFixed()}%
                            </div>
                        </div>
                    </div>
                    <div className="graph">
                        <p className="graph-text">A graph will be displayed here</p>
                    </div>
                </div>
                <div className="time-words-characters-startTime-endTime">
                    <div className="time">
                        <div className="bottom-text">
                            <p className="paragraph">LAIKAS</p>
                        </div>
                        <div className="bottom-number">
                            {completionTimeInSeconds}s
                        </div>
                    </div>
                    <div className="words">
                        <div className="bottom-text">
                            <p className="paragraph">ŽODŽIAI</p>
                        </div>
                        <div className="bottom-number">
                            {gameData.TotalAmountOfWords}
                        </div>
                    </div>
                    <div className="characters">
                        <div className="bottom-text">
                            <p className="paragraph">IŠ VISO/KLAIDOS</p>   
                        </div>
                        <div className="bottom-number">
                            {gameData.TotalAmountOfCharacters}/{gameData.NumberOfWrongfulCharacters}
                        </div>
                    </div>
                    <div className="startTime">
                        <div className="bottom-text">
                            <p className="paragraph">PRADŽIA</p>
                        </div>
                        <div className="bottom-number">
                            {formattedStartTime}
                        </div>
                    </div>
                    <div className="endTime">
                        <div className="bottom-text">
                            <p className="paragraph">PABAIGA</p>
                        </div>
                        <div className="bottom-number">
                            {formattedFinishTime}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default GameData;