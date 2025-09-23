import matplotlib.pyplot as plt
from sklearn.linear_model import LinearRegression

import numpy as np


def temp_1():
    temp_read = np.array([23, 26, 30, 32, 35, 38, 39, 40, 40.5, 42]).reshape(-1, 1)
    temp_actu = np.array([62, 65, 72, 76, 80, 80, 82, 84, 86, 87]).reshape(-1, 1)

    reg = LinearRegression().fit(temp_read, temp_actu)

    x = np.linspace(1, 100, 25).reshape(-1, 1)
    y = reg.predict(x)

    plt.figure()
    plt.title("gpu temp")
    plt.xlabel("value read")
    plt.ylabel("actual value")
    plt.scatter(temp_read, temp_actu)
    plt.plot(x, y)
    plt.show()


def temp_0():
    temp_read = np.array([24, 30, 33, 35, 37, 40, 42, 44]).reshape(-1, 1)
    temp_actu = np.array([27, 58, 62, 64, 65.5, 69, 70.5, 72]).reshape(-1, 1)

    reg = LinearRegression().fit(temp_read, temp_actu)

    x = np.linspace(1, 100, 25).reshape(-1, 1)
    y = reg.predict(x)

    plt.figure()
    plt.title("CPU temp")
    plt.xlabel("value read")
    plt.ylabel("actual value")
    plt.scatter(temp_read, temp_actu)
    plt.plot(x, y)
    plt.show()


if __name__ == "__main__":
    temp_0()
    # temp_1()
