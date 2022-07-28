import os
import keras
from keras import layers
from keras import models
from keras import optimizers
from keras.preprocessing.image import ImageDataGenerator
import matplotlib.pyplot as plt
from keras.callbacks import ModelCheckpoint

from PIL import ImageFile
ImageFile.LOAD_TRUNCATED_IMAGES = True

BaseDir = '/Users/trevor-trou/Documents/Git/snow-model'

TrainDir = os.path.join(BaseDir, 'Train')
TestDir = os.path.join(BaseDir, 'Test')
ValidationDir = os.path.join(BaseDir, 'Validation')

dataGen = ImageDataGenerator(rescale = 1. / 255)
train_generator = dataGen.flow_from_directory(TrainDir, target_size = (150, 150), batch_size=32, class_mode='binary')
test_generator = dataGen.flow_from_directory(TestDir, target_size = (150, 150), batch_size=32, class_mode='binary')
validation_generator = dataGen.flow_from_directory(ValidationDir, target_size = (150, 150), batch_size=32, class_mode='binary')

# Examine output
# for data_batch, labels_batch in train_generator:
#     print("data batch shape: ", data_batch.shape)
#     print("labels batch shape: ", labels_batch.shape)
#     break

#Model Architecture
model = models.Sequential()

# First convolution layer
model.add(layers.Conv2D(32, (3,3), activation = 'relu', input_shape=(150,150,3)))
model.add(layers.MaxPool2D((2,2)))

# Second convolution layer
model.add(layers.Conv2D(64, (3,3), activation = 'relu'))
model.add(layers.MaxPool2D((2,2)))

# Third convolution layer
model.add(layers.Conv2D(128, (3,3), activation = 'relu'))
model.add(layers.MaxPool2D((2,2)))



#Fully Connected or Densely Connected Classifier Network
model.add(layers.Flatten()) # Flatten 3D output to 1D
model.add(layers.Dropout(0.5))
model.add(layers.Dense(512, activation = 'relu'))

model.add(layers.Dense(256, activation = 'relu'))

#Output layer with a single neuron since it is a binary class problem
model.add(layers.Dense(1, activation = 'sigmoid'))
model.summary()

#Configure the model for running
model.compile(loss = 'binary_crossentropy', optimizer = optimizers.RMSprop(1e-04), metrics = ['acc'])

filepath="weights.best.h5"
checkpoint = ModelCheckpoint(filepath, monitor='val_loss', mode='min', verbose=1, save_best_only=True)
callbacks_list = [checkpoint]

#Train the Model: Fit the model to the Train Data using a batch generator
history = model.fit_generator(train_generator, steps_per_epoch=52, epochs=24, validation_data = validation_generator, callbacks=callbacks_list)
#history = model.fit_generator(train_generator, epochs=30, validation_data = validation_generator)

#Saving the Trained Model
#model.save('snow-model.h5')



#Plotting the loss and accuracy
acc = history.history['acc']
val_acc = history.history['val_acc']
loss = history.history['loss']
val_loss = history.history['val_loss']

epochs = range(len(acc))
plt.plot(epochs,acc,'b', label = 'Training Accuracy')
plt.plot(epochs,val_acc,'r',label = 'Validation Accuracy')
plt.title('Training and Validation Accuracy')
plt.legend()
plt.show()


plt.plot(epochs,loss,'b', label = 'Training loss')
plt.plot(epochs,val_loss,'r',label = 'Validation loss')
plt.title('Training and Validation loss')
plt.legend()
plt.show()

# Testing
test_loss, test_acc = model.evaluate_generator(test_generator, steps = 50)
print("Test Accuracy = ", test_acc)