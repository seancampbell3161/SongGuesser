import React, { useState } from 'react';
import axios from 'axios';
import ReactAudioPlayer from 'react-audio-player';
import { Oval } from 'react-loader-spinner'; 

const YouTubeConverter = () => {
    const [url, setUrl] = useState('');
    const [message, setMessage] = useState('');
    const [tracks, setTracks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [songTitle, setSongTitle] = useState('');
    const [artist, setArtist] = useState('');

    const handleUrlChange = (e) => {
        setUrl(e.target.value);
    };

    const handleSongTitleChange = (e) => {
        setSongTitle(e.target.value);
    }

    const handleArtistChange = (e) => {
        setArtist(e.target.value);
    }

    const handleConvert = async () => {
        setLoading(true);
        setMessage('');
        setTracks([]);

        if (!url || !songTitle || !artist) {
            setMessage('You must enter a URL, song title, and artist name');
            setLoading(false);
            return;
        }

        try {
            const response = await axios.post('http://localhost:5244/api/Music/convert-and-separate', { url, songTitle, artist }, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            setMessage(response.data.message);
            setTracks(response.data.tracks);
        } catch (error) {
            setMessage('Error converting YouTube video');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <input type="text" value={url} onChange={handleUrlChange} placeholder="YouTube URL" />         
            <form>
                <input type="text" value={songTitle} onChange={handleSongTitleChange} placeholder="Song Title"></input>
                <br />
                <input type="text" value={artist} onChange={handleArtistChange} placeholder="Artist"></input>
            </form>
            <button onClick={handleConvert}>Add Song</button>
            
            {loading && (
                <div className="spinner">
                    <Oval color="#00BFFF" height={80} width={80} /> {/* Spinner component */}
                </div>
            )}

            <p>{message}</p>
            
            {tracks.length > 0 && (
                <div>
                    {tracks.map((track, index) => (
                        <div key={index}>
                            <h4>{track.name}</h4>
                            <ReactAudioPlayer src={`http://localhost:5244${track.url}`} controls />
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default YouTubeConverter;