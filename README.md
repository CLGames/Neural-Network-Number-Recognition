This is a simple Multilayer Perceptron (MLP) written completely from scratch and trained on the MNIST dataset to recognise handwritten digits that the user draws into the UI.

<img width="781" height="388" alt="image" src="https://github.com/user-attachments/assets/828ae89b-efbd-4c9c-b047-aeecb13c2941" />

This neural network model features a 784 neuron input layer (to match with MNIST image size), a 128 neuron hidden layer, and a 10 neuron output layer (corresponding to the 10 possible digits). ReLU is used for the hidden layer activation function and Softmax for the output.

The model was first trained to 98.03% accuracy on the MNIST dataset (accuracy tested on the MNIST testing data), then I created a basic windows forms application to allow a user to draw in a digit and have the neural network predict which digit was drawn.

From my tests, the model is pretty decent at recognising user inputs, though it struggles with the number 7 - often confusing 7 for 2 or 3. This could be due to the way the number 7 is represented across the training data or maybe more measures could be taken to normalise the user input to a form that more closely represents the MNIST training data images.

Written in C# in Visual Studio 2022
