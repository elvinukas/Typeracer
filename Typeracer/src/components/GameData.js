import React, { useEffect, useState } from "react";
import '../../wwwroot/css/GameData.css';
import Leaderboard from './Leaderboard';
function GameData( { gameId }) {
    const [gameData, setGameData] = useState(null);
    const [paragraphData, setParagraphData] = useState(null);
    
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
        fetch(`api/Game/${gameId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Game data not found");
                }
                return response.json();
            })
            .then(data => {
                console.log("Fetched game data:", data);
                setGameData(data);

                if (data.statistics.paragraphId) {
                    fetchParagraphData(data.statistics.paragraphId);
                }
                
            })
            .catch(error => console.error("Error fetching game data:", error));
    }, [gameId]);
    
    
    function fetchParagraphData(paragraphId) {
        fetch(`/api/paragraphs/${paragraphId}`)
        .then(response => {
            if (!response.ok) {
                console.log("Failed to retrieve paragraph data");
                throw new Error("Paragraph data not found");
            }
            return response.json();
        })
            .then(data => {
                console.log("Fetched paragraph data:", data);
                setParagraphData(data); // Save the paragraph data
            })
            .catch(error => console.error("Error fetching paragraph data:", error));
    }

    if (showLeaderboard) {
        return <Leaderboard />;
    }

    if (!gameData) {
        return <div>Loading...</div>; //loading screen
    }

    if (!gameData) {
        return <div>Loading game data...</div>; // loading screen for game data
    }

    if (!paragraphData) {
        return <div>Loading paragraph data...</div>; // loading screen for paragraph data
    }
    
    console.log("This is the localStartTime: ", gameData.statistics.localStartTime);
    console.log("This is totalAmountOfWords: ", paragraphData.totalAmountOfWords);
    
    
    const startTime = new Date(gameData.statistics.localStartTime);
    const finishTime = new Date(gameData.statistics.localFinishTime);
    const completionTimeInSeconds = ((finishTime - startTime) / 1000).toFixed(2);

    const formattedStartTime = startTime.toLocaleTimeString('en-GB', { hour12: false });
    const formattedFinishTime = finishTime.toLocaleTimeString('en-GB', { hour12: false });
    
    const wordsPerMinute = gameData.statistics.wordsPerMinute|| "N/A";
    const accuracy = gameData.statistics.accuracy || "N/A";
    
    const saveStatistics = async () => {
        const username = prompt("Įveskite savo vartotojo vardą:");
        if (!username) {
            alert("Vartotojo vardas būtinas!");
            return;
        }
        
        //console.log("Generated or retrieved PlayerID: ", playerID);

        const playerData = {
            Username: username,
            BestWPM: gameData.statistics.wordsPerMinute,
            BestAccuracy: gameData.statistics.accuracy,
            GameId: gameId
        };

        console.log('Sending player and associated gameID data:', playerData);

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
                            {paragraphData.totalAmountOfWords}
                        </div>
                    </div>
                    <div className="characters">
                        <div className="bottom-text">
                            <p className="paragraph">IŠ VISO/KLAIDOS</p>
                        </div>
                        <div className="bottom-number">
                            {paragraphData.totalAmountOfCharacters}/{gameData.statistics.numberOfWrongfulCharacters}
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