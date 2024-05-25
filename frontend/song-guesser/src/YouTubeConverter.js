import React, { useState } from 'react';
import axios from 'axios';
import ReactAudioPlayer from 'react-audio-player';

const YouTubeConverter = () => {
    const [url, setUrl] = useState('');
    const [message, setMessage] = useState('');
    const [mp3Url, setMp3Url] = useState('');

    const handleUrlChange = (e) => {
        setUrl(e.target.value);
    };

    const handleConvert = async () => {
        const formData = new FormData();
        formData.append('url', url);

        try {
            const response = await axios.post('http://localhost:5244/api/music/youtube', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            setMessage(response.data.message);
            setMp3Url(response.data.url);
        } catch (error) {
            setMessage('Error converting YouTube video');
        }
    };

    return (
        <div>
            <input type="text" value={url} onChange={handleUrlChange} placeholder="Enter YouTube URL" />
            <button onClick={handleConvert}>Convert</button>
            <p>{message}</p>
            {mp3Url && (
                <div>
                    <h4>Converted MP3</h4>
                    <ReactAudioPlayer src={`http://localhost:5244${mp3Url}`} controls />
                </div>
            )}
        </div>
    );
};

export default YouTubeConverter;