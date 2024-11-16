import React, { useEffect, useState, useRef } from 'react';
import axios from 'axios';
import ConvertPage from './ConvertPage';
import './App.css';

const stages = [
    { name: 'Drums', tracks: ['drums'] },
    { name: 'Drums + Bass', tracks: ['drums', 'bass'] },
    { name: 'Drums + Bass + Other', tracks: ['drums', 'bass', 'other'] },
    { name: 'Drums + Bass + Other + Vocals', tracks: ['drums', 'bass', 'other', 'vocals'] }
];

const HomePage = () => {
    const [tracks, setTracks] = useState([]);
    const [message, setMessage] = useState('');
    const [currentStage, setCurrentStage] = useState(0);
    const [songGuess, setSongGuess] = useState('');
    const [artistGuess, setArtistGuess] = useState('');
    const [feedback, setFeedback] = useState('');
    const [correctSong, setCorrectSong] = useState('');
    const [correctArtist, setCorrectArtist] = useState('');
    const [isPlaying, setIsPlaying] = useState(false);
    const audioRefs = useRef({});

    useEffect(() => {
        const fetchRandomSong = async () => {
            try {
                const response = await axios.get('http://localhost:5244/api/Music/random');
                setTracks(response.data.tracks);
                // setMessage(`Playing a random song...`);
                setCorrectSong(response.data.title);
                setCorrectArtist(response.data.artist);
                response.data.tracks.forEach(track => {
                    audioRefs.current[track.name] = React.createRef();
                });
            } catch (error) {
                setMessage('Error fetching random song');
            }
        };

        fetchRandomSong();
    }, []);

    const playTracks = (trackNames) => {
        Object.values(audioRefs.current).forEach(ref => {
            if (ref.current) {
                ref.current.pause();
                ref.current.currentTime = 0;
            }
        });

        trackNames.forEach(trackName => {
            const audioEl = audioRefs.current[trackName].current;
            if (audioEl) {
                audioEl.play();
            }
        });

        setIsPlaying(true);
    };

    const pauseTracks = () => {
        Object.values(audioRefs.current).forEach(ref => {
            if (ref.current) {
                ref.current.pause();
            }
        });

        setIsPlaying(false);
    };

    const handleGuess = () => {
        let feedbackMessage = '';
        if (songGuess.toLowerCase() === correctSong.toLowerCase()) {
            feedbackMessage += 'Correct song title! ';
        } else {
            feedbackMessage += 'Incorrect song title. ';
        }

        if (artistGuess.toLowerCase() === correctArtist.toLowerCase()) {
            feedbackMessage += 'Correct artist name!';
        } else {
            feedbackMessage += 'Incorrect artist name.';
        }

        setFeedback(feedbackMessage);
    };

    const handleNextStage = () => {
        pauseTracks();
        if (currentStage < stages.length - 1) {
            setCurrentStage(currentStage + 1);
            setFeedback('');
        }
    };

    return (
        <div className="home">
            <h1>Guess the Song</h1>
            <p>{message}</p>
            {tracks.length > 0 && (
                <div>
                    {tracks.map((track, index) => (
                        <div key={index}>
                            {/* <h4>{track.name}</h4> */}
                            <audio ref={audioRefs.current[track.name]} src={`http://localhost:5244${track.path}`} />
                        </div>
                    ))}
                </div>
            )}
            {tracks.length > 0 && (
                <div className="audio-options">
                    <h2>Currently Playing: </h2>
                    <h2>{stages[currentStage].name}</h2>
                    <div className="audio-btns">
                        <button onClick={() => playTracks(stages[currentStage].tracks)} disabled={isPlaying}>Play</button>
                        <button onClick={pauseTracks} disabled={!isPlaying}>Stop</button>
                        <button onClick={handleNextStage} disabled={currentStage >= stages.length - 1}>Next</button>
                    </div>                
                </div>
            )}
            <div className="guessing-fields">
                <input
                    type="text"
                    placeholder="Song Title"
                    value={songGuess}
                    onChange={(e) => setSongGuess(e.target.value)}
                />
                <input
                    type="text"
                    placeholder="Artist"
                    value={artistGuess}
                    onChange={(e) => setArtistGuess(e.target.value)}
                />
                <button onClick={handleGuess}>Confirm</button>
            </div>
            {feedback && <p>{feedback}</p>}

            <ConvertPage />
        </div>
    );
};

export default HomePage;