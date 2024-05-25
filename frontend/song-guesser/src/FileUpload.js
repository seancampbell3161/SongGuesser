import React, { useState } from 'react';
import axios from 'axios';
import ReactAudioPlayer from 'react-audio-player';

const FileUpload = () => {
    const [file, setFile] = useState(null);
    const [message, setMessage] = useState('');
    const [tracks, setTracks] = useState([]);

    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    const handleUpload = async () => {
        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await axios.post('http://localhost:5244/api/music/upload', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            setMessage(response.data.message);
            const downloadUrl = response.data.downloadUrl;
            const trackNames = ['vocals', 'drums', 'bass', 'other'];
            setTracks(trackNames.map(name => `${downloadUrl}/${name}.wav`));
        } catch (error) {
            setMessage('Error uploading file');
        }
    };

    return (
        <div>
            <input type="file" onChange={handleFileChange} />
            <button onClick={handleUpload}>Upload</button>
            <p>{message}</p>
            {tracks.length > 0 && (
                <div>
                    {tracks.map((track, index) => (
                        <div key={index}>
                            <h4>{track.split('/').pop()}</h4>
                            <ReactAudioPlayer src={`http://localhost:5000/${track}`} controls />
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default FileUpload;