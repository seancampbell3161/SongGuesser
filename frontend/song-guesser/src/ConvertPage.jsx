import React, { useState } from 'react';
import axios from 'axios';
import ReactAudioPlayer from 'react-audio-player';
import { Oval } from 'react-loader-spinner';

const ConvertPage = () => {
    const [url, setUrl] = useState('');
    const [message, setMessage] = useState('');
    const [tracks, setTracks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [artist, setArtist] = useState('');
    const [songTitle, setSongTitle] = useState('');

    const handleSongTitleChange = (e) => {
        setSongTitle(e.target.value);
    }

    const handleAristChange = (e) => {
        setArtist(e.target.value);
    }
    const handleUrlChange = (e) => {
        setUrl(e.target.value);
    };

    const handleConvert = async () => {
        setLoading(true);
        setMessage('');
        setTracks([]);

        const request = {
            songTitle: songTitle,
            artist: artist,
            url: url
        }

        try {
            const response = await axios.post('http://localhost:5244/api/Music/convert-and-separate', request, {
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
            <input type="text" value={songTitle} onChange={handleSongTitleChange} placeholder="Song Title" />
            <input type="text" value={artist} onChange={handleAristChange} placeholder="Artist Name" />
            <input type="text" value={url} onChange={handleUrlChange} placeholder="Enter YouTube URL" />
            <button onClick={handleConvert}>Convert</button>
            
            {loading && (
                <div className="spinner">
                    <Oval color="#00BFFF" height={80} width={80} />
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

export default ConvertPage;