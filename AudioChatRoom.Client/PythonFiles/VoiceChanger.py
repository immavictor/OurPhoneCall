import pyaudio
import numpy as np
from scipy import signal
cutoff_frequency = 1000  # 截止频率为1000 Hz
filter_order = 4  # 滤波器阶数

# 创建低通滤波器
b, a = signal.butter(filter_order, cutoff_frequency, 'low', fs=44100)
sample_rate = 44100
frame_size = 1024

pitch_factor = 1.31

p = pyaudio.PyAudio()

stream_in = p.open(format=pyaudio.paFloat32,
                   channels=1,
                   rate=sample_rate,
                   input=True,
                   frames_per_buffer=frame_size)

tream_out = p.open(format=pyaudio.paFloat32,
                   channels=1,
                   rate=sample_rate,
                   output=True,
                   frames_per_buffer=frame_size)
try:
    while True:
        data_in = stream_in.read(frame_size)

        audio_in = np.frombuffer(data_in, dtype=np.float32)


        audio_out = signal.resample_poly(audio_in, int(1.63 * pitch_factor), int(2.35 / pitch_factor))

        data_out = audio_out.astype(np.float32).tobytes()

        tream_out.write(data_out)

except KeyboardInterrupt:
    print("程序已手动终止，执行清理操作...")

    # 停止音频流
    tream_out.stop_stream()
    stream_in.stop_stream()

    # 关闭音频流
    tream_out.close()
    stream_in.close()

    # 终止 PyAudio 对象
    p.terminate()