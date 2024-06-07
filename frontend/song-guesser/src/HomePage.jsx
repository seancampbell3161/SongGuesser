import React, { useEffect, useState, useRef } from 'react';
import axios from 'axios';

const HomePage = () => {
    const [tracks, setTracks] = useState([]);
    const [message, setMessage] = useState('');
    const audioRefs = useRef({}); // Refs to control each audio element

    useEffect(() => {
        const fetchRandomSong = async () => {
            try {
                const response = await axios.get('http://localhost:5244/api/Music/random');
                setTracks(response.data.tracks);
                setMessage(response.data.message);
                // Initialize audio refs
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
        // Pause all tracks first
        Object.values(audioRefs.current).forEach(ref => {
            if (ref.current) {
                ref.current.pause();
                ref.current.currentTime = 0;
            }
        });

        // Play selected tracks
        trackNames.forEach(trackName => {
            const audioEl = audioRefs.current[trackName].current;
            if (audioEl) {
                audioEl.play();
            }
        });
    };

    const pauseTracks = () => {
        Object.values(audioRefs.current).forEach(ref => {
            if (ref.current) {
                ref.current.pause();
            }
        });
    };

    return (
        <div>
            <h1>Random Song</h1>
            <p>{message}</p>
            {tracks.length > 0 && (
                <div>
                    {tracks.map((track, index) => (
                        <div key={index}>
                            <h4>{track.name}</h4>
                            <audio ref={audioRefs.current[track.name]} src={`http://localhost:5244/${track.url}`} />
                        </div>
                    ))}
                </div>
            )}
            {tracks.length > 0 && (
                <div>
                    <button onClick={() => playTracks(['drums'])}>Play Drums</button>
                    <button onClick={() => playTracks(['drums', 'bass'])}>Play Drums and Bass</button>
                    <button onClick={() => playTracks(['drums', 'bass', 'other'])}>Play Drums, Bass, and Other</button>
                    <button onClick={() => playTracks(['drums', 'bass', 'other', 'vocals'])}>Play All Tracks</button>
                    <button onClick={pauseTracks}>Pause</button>
                </div>
            )}
        </div>
    );
};

export default HomePage;