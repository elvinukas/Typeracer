import React, { useEffect, useState } from "react";
import '../../wwwroot/css/GameData.css';
import Leaderboard from './Leaderboard';
function GameData() {
    const [gameData, setGameData] = useState(null);
    
    const [showLeaderboard, setShowLeaderboard] = useState(false);

    const [appID, setAppID] = useState(null);

    useEffect(() => {
        // Fetch the application ID from the server
        fetch('/api/application-id')
            .then(response => response.text())
            .then(id => {
                setAppID(id);
                handleAppID(id);
            })
            .catch(error => console.error('Error fetching application ID:', error));
    }, []);

    function handleAppID(currentAppID) {
        const storedAppID = localStorage.getItem('applicationID');

        if (storedAppID !== currentAppID) {
            // Application has restarted or updated
            // Clear stored playerID and update applicationID
            localStorage.removeItem('playerID');
            localStorage.setItem('applicationID', currentAppID);
        }
    }
    

    useEffect(() => {
        fetch('/statistics/game-data.json')
            .then(response => response.json())
            .then(data => setGameData(data))
            .catch(error => console.error('Error fetching game data:', error));
    }, []);

    if (showLeaderboard) {
        return <Leaderboard />;
    }

    if (!gameData) {
        return <div>...</div>; //loading screen
    }
    
    const completionTimeInSeconds = (gameData.CompletionTime).toFixed(2);
    const startTime = new Date(gameData.Statistics.LocalStartTime);
    const formattedStartTime = startTime.toLocaleTimeString('en-GB', { hour12: false });
    const finishTime = new Date(gameData.Statistics.LocalFinishTime);
    const formattedFinishTime = finishTime.toLocaleTimeString('en-GB', { hour12: false });
    
    const wordsPerMinute = gameData.CalculativeStatistics.WordsPerMinute;
    const accuracy = gameData.CalculativeStatistics.Accuracy;

    function generateUUID() {
        // generating a random playerID
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            const r = Math.random() * 16 | 0;
            const v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    const saveStatistics = async () => {
        const username = prompt("Įveskite savo vartotojo vardą:");
        if (!username) {
            alert("Vartotojo vardas būtinas!");
            return;
        }

        let playerID = localStorage.getItem('playerID');
        if (!playerID) {
            playerID = generateUUID();
            localStorage.setItem('playerID', playerID);
        }
        console.log("Generated or retrieved PlayerID: ", playerID);

        const playerData = {
            PlayerID: playerID,
            Username: username,
            BestWPM: gameData.CalculativeStatistics.WordsPerMinute,
            BestAccuracy: gameData.CalculativeStatistics.Accuracy
        };

        console.log('Sending player data:', playerData);

        try {
            const response = await fetch('/api/leaderboard/save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(playerData)
            });

            if (!response.ok) {
                throw new Error('Nepavyko išsaugoti duomenų lyderių lentelėje');
            }

            alert('Rezultatai išsaugoti!');
        } catch (error) {
            console.error('Klaida:', error);
        }
    };
    
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
                                {wordsPerMinute.toFixed()}
                            </div>
                        </div>
                        <div className="acc">
                            <div className="top-text">
                                <p className="paragraph">TIKS.</p>
                            </div>
                            <div className="top-number">
                                {accuracy.toFixed()}%
                            </div>
                        </div>
                    </div>
                    <div className="graph">
                        <img src="/images/wpm-graph.png" alt="WPM Graph"/>
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
                            {gameData.Statistics.Paragraph.TotalAmountOfWords}
                        </div>
                    </div>
                    <div className="characters">
                        <div className="bottom-text">
                            <p className="paragraph">IŠ VISO/KLAIDOS</p>
                        </div>
                        <div className="bottom-number">
                            {gameData.Statistics.Paragraph.TotalAmountOfCharacters}/{gameData.Statistics.NumberOfWrongfulCharacters}
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
            <div className="text-center">
                <button className="btn btn-primary btn-lg mt-3" style={{ marginRight: '10px' }} onClick={saveStatistics}>Išsaugoti statistiką</button>
                <button className="btn btn-primary btn-lg mt-3" onClick={() => setShowLeaderboard(true)}>
                    Peržiūrėti lyderių lentelę
                </button>
            </div>

        </div>
    );
}

export default GameData;