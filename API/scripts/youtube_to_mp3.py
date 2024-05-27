import yt_dlp
import sys
import os

def sanitize_filename(filename):
    return filename.replace(' ', '_')

def download_audio(youtube_url, output_dir):
    ydl_opts = {
        'format': 'bestaudio/best',
        'postprocessors': [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': 'mp3',
            'preferredquality': '192',
        }],
        'outtmpl': os.path.join(output_dir, '%(title)s.%(ext)s')
    }
    
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        info_dict = ydl.extract_info(youtube_url, download=True)
        title = info_dict.get('title', None)
        if title:
            original_filename = os.path.join(output_dir, f"{title}.mp3")
            sanitized_filename = os.path.join(output_dir, sanitize_filename(f"{title}.mp3"))
            if os.path.exists(original_filename):
                os.rename(original_filename, sanitized_filename)
                print(sanitized_filename) # capture filename
                return sanitized_filename
    return None

if __name__ == "__main__":
    youtube_url = sys.argv[1]
    output_dir = sys.argv[2]
    download_audio(youtube_url, output_dir)