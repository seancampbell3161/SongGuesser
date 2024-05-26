import yt_dlp
import sys
import os
from spleeter.separator import Separator

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
        info_dict = ydl.extract_info(youtube_url, download=True)
        return ydl.prepare_filename(info_dict)

def separate_tracks(input_file, output_dir):
    separator = Separator('spleeter:4stems')
    separator.separate_to_file(input_file, output_dir)

if __name__ == "__main__":
    youtube_url = sys.argv[1]
    mp3_output_dir = sys.argv[2]
    wav_output_dir = sys.argv[3]
    
    mp3_file = download_audio(youtube_url, mp3_output_dir)
    separate_tracks(mp3_file, wav_output_dir)