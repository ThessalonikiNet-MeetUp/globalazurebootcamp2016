Azure iot 


Settings Rasperry pi
connect pi to network
connect pc from web  (8080)  // p@ssw0rd2016
change machine to development mode
Connect to remote (configure ip)


Demo
Create class measurement (just temperature)
Create a timer

(Show dependencies from extensions and about)
Show project.json

Connect to azure portal

usefull extension Connected Service for Azure IoT Hub 

Right click on the project, select “Add” -> “Connected Service”

Click “Install” and this will be installed in your project
We’ll proceed by adding a reference to the Azure IoT Hub. This will generate some code for our device to connect.
Right click on the project, select “Add” -> “Connected Service”
Select “Azure IoT Hub” and press “Configure”
Enter your Azure account credentials
Select the account that contains the Azure IoT Hub that you previously created and press “Add”
It will prompt you to select a device or create a new one. Let’s create a new one, I’ll call mine “myraspberry”. Enter the name and press “Add”.
You’ll see a new file added to the project, called “AzureIoTHub.cs”. This file contains some useful code to send and receive messages from Azure IoT Hub.
We’ll need a class to store the messages we’ll send to Azure IoT Hub. Create a new class, call it “SenseHatData” and paste the following code

https://github.com/Azure/azure-iot-hub-vs-cs/wiki/C%23-Usage



Create a new Azure SQL Database

-table

On the Azure portal, select to create a new “Stream Analytics job”
 