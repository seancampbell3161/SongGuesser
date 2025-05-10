import wave
import numpy as np
import sys
import json


def generate_waveform_data(path):
    file = wave.open(path, 'rb')
    sample_freq = file.getframerate()
    n_samples = file.getnframes()

    signal_wave = file.readframes(n_samples)

    signal_array = np.frombuffer(signal_wave, dtype=np.int16)
    l_channel = signal_array[0::2]
    amp_int = list(map(int, l_channel))

    timestamps = np.linspace(0, n_samples/sample_freq, num=n_samples)
    data = [{'time': t, 'amplitude': a} for t, a in zip(timestamps, amp_int)]
    print(json.dumps(data))

    file.close()


if __name__ == '__main__':
    file_path = sys.argv[1]
    generate_waveform_data(file_path)
