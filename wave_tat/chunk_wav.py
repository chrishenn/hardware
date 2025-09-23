import os
import glob
from pydub import AudioSegment
import time

import multiprocessing as mp

from multiprocessing import Value, Lock, Queue


def cut_chunks(queue: Queue):
    target_length = 3

    while not queue.empty():
        file_name = queue.get()
        song = AudioSegment.from_wav(file_name + ".wav")

        for i in range(len(song) // (target_length * 1000)):
            seconds = float(target_length) * 1000 * i
            seconds2 = float(target_length) * 1000 * (i + 1)
            cut = song[int(seconds) : int(seconds2)]
            cut.export(file_name + "_chunk_" + str(i) + ".wav", format="wav")


def del_by_size(queue: Queue):
    count = 0
    while not queue.empty():
        count += 1
        if count % 300 == 0:
            print("visited 300")

        file_name = queue.get()
        song = AudioSegment.from_wav(file_name + ".wav")

        if len(song) < 2995 or len(song) > 3005:
            os.remove(file_name + ".wav")

            with lock:
                del_counter.value += 1


if __name__ == "__main__":
    q: Queue = mp.Queue()

    os.chdir("./data")
    tic = time.time()
    print("create queue start")
    for name in glob.glob("*.wav"):
        q.put(name.split(".")[0])
    print("queue create time: ", time.time() - tic, " sec")

    del_counter = Value("i", 0)
    lock = Lock()

    # procs = [mp.Process(target=cut_chunks, args=(queue,)) for i in range(12)]
    # procs = [mp.Process(target=del_by_size, args=(queue, )) for i in range(12)]

    del_by_size(q)

    # for p in procs: p.start()
    print("chunking started")
    # for p in procs: p.join()
    print("chunking done")
    print("files deleted: ", del_counter.value())
