# ThirdPerson_Male_Avatar
 Third Person avatar with Adobe Maximo Animations


Animations:
-Idle
-Walking
-Running
-Turning
-Jumping
-Crouching


Initialization:
-Make sure you have Cinemachine plugins installed
-Add CinemachineBrain component to the Main Camera
-Create "Ground" Layer and set it as the layer of the ground in your environment.
-Place "Male_Prototype" prefab on the ground
	Under Third Person Movement Script:
	-Set Cam attribute to the Main Camera
-Place CM FreeLook1 prefab
	Under CinemachineFreeLook:
	-Set Follow attribute to follow the Armature of the "Male_Prototype"
	-Set Look At attribute to look at Head of the "Male_Prototype"
And Enjoy :)
