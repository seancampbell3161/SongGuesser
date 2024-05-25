import subprocess
import sys
import logging
import os
from spleeter.separator import Separator

def check_ffmpeg():
    try:
        subprocess.run(["/opt/homebrew/bin/ffmpeg", "-version"], check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        print("ffmpeg is installed and accessible.")
    except subprocess.CalledProcessError:
        print("ffmpeg is not installed or not accessible.")
        sys.exit(1)

def separate_tracks(input_file, output_dir):
    os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'  # 0 = all logs, 1 = info, 2 = warnings, 3 = errors
    logging.getLogger('tensorflow').setLevel(logging.ERROR)
    
    separator = Separator('spleeter:4stems')
    separator.separate_to_file(input_file, output_dir)

if __name__ == "__main__":
    input_file = sys.argv[1]
    output_dir = sys.argv[2]
    check_ffmpeg()
    separate_tracks(input_file, output_dir)