import yt_dlp
import sys
import os

def download_audio(youtube_url, output_dir):
    ydl_opts = {
        'format': 'bestaudio/best',
        'postprocessors': [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': 'mp3',
            'preferredquality': '192',
        }],
        'outtmpl': os.path.join(output_dir, '%(title)s.%(ext)s'),
    }
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        ydl.download([youtube_url])

if __name__ == "__main__":
    youtube_url = sys.argv[1]
    output_dir = sys.argv[2]
    download_audio(youtube_url, output_dir)