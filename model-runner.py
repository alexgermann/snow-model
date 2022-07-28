import os, shutil
import keras
from keras import layers
from keras import models
from keras.models import load_model
from keras.preprocessing import image #Library for preprocessing the image into a 4D tensor
import numpy as np
import matplotlib.pyplot as plt
from PIL import ImageFile
ImageFile.LOAD_TRUNCATED_IMAGES = True

import re
import csv

float_formatter = lambda x: "%.4f" % x

BaseDir = '/Users/trevor-trou/Documents/Git/snow-model/Test'

snowPath = os.path.join(BaseDir, 'Snow')
noSnowPath = os.path.join(BaseDir, 'Non')

snowImageNames = os.listdir(snowPath)
noSnowImageNames = os.listdir(noSnowPath)

jpg = re.compile('.*.jpg')

for img in snowImageNames:
    if not jpg.match(img):
        snowImageNames.remove(img)

for img in noSnowImageNames:
    if not jpg.match(img):
        noSnowImageNames.remove(img)

model = load_model('weights.best.h5')

# Clear screen
print(chr(27) + "[2J")

csvfile = open('test_results.csv', 'w', newline='')
writer = csv.DictWriter(csvfile, fieldnames=['file', 'result'])
writer.writeheader()

print("Evaluating Snow Images...")
misclass = 0
for i in range(0, len(snowImageNames)):
    imgPath = os.path.join(snowPath, snowImageNames[i])
    img = image.load_img(imgPath,target_size=(150,150))

    img_tensor = image.img_to_array(img)
    img_tensor = np.expand_dims(img_tensor,axis=0)
    img_tensor /=255.

    #Plot the test image
    # plt.imshow(img_tensor[0])
    # plt.show()

    prediction = model.predict(img_tensor)
    res = prediction[0][0]
    writer.writerow({'file': snowImageNames[i], 'result': float_formatter(res)})
    if res < 0.5:
        print(float_formatter(res)+ "\t" + snowImageNames[i])
        misclass += 1

print("Misclassified: {0} of {1} (Rate: {2:0.4f}%)".format(misclass, len(snowImageNames), misclass / len(snowImageNames)))

print("\n\n")
print("Evaluating No Snow Images...")
misclass = 0
writer.writerow({'file': '', 'result': ''})
for i in range(0, len(noSnowImageNames)):
    imgPath = os.path.join(noSnowPath, noSnowImageNames[i])
    img = image.load_img(imgPath,target_size=(150,150))

    img_tensor = image.img_to_array(img)
    img_tensor = np.expand_dims(img_tensor,axis=0)
    img_tensor /=255.

    #Plot the test image
    # plt.imshow(img_tensor[0])
    # plt.show()

    prediction = model.predict(img_tensor)
    res = prediction[0][0]
    writer.writerow({'file': noSnowImageNames[i], 'result': float_formatter(res)})
    if res > 0.5:
        print(float_formatter(prediction[0][0]) + "\t" + noSnowImageNames[i])
        misclass += 1

print("Misclassified: {0} of {1} (Rate: {2:0.4f}%)".format(misclass, len(noSnowImageNames), misclass / len(noSnowImageNames)))