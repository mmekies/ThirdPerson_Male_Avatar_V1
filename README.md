<h1>Third Person Animated Avatar</h1>

<h3>Third Person Avatar with Adobe Maximo Animations</h3>

<dl>
	<dt>Animations included:</dt>
	<dd>-Idle</dd>
	<dd>-Walking</dd>
	<dd>-Running</dd>
	<dd>-Turning</dd>
	<dd>-Jumping</dd>
	<dd>-Crouching</dd>
</dl>

<h3>Initialization</h3>

<dl>
    <dt>-Make sure you have Cinemachine plugins installed.</dt>
    <dt>-Create "Ground" Layer and set it as the layer of the ground in your environment.</dt>
    <dt>-Place "Male_Prototype" prefab on the ground.</dt>
    <dd>Under Third Person Movement Script:</dd>
    <dd>-Set Cam attribute to the Main Camera</dd>
    <dd>-Set Anim attribute to Male_Prototype_V1</dd>
    <dd>-Set Turn Smooth Timer to 0.1</dd>
    <dd>-Set Speed to 6</dd>
    <dd>-Set Mass to 5</dd>
    <dd>-Set Ground Check Distance to 0.01</dd>
    <dd>-Set Ground Mask to Ground</dd>
    <dd>-Set Gravity to -9.81</dd>
    <dd>-Set Jump Height to 8</dd>
    <dt>-Place CM FreeLook1 prefab</dt>
    <dd>Under CinemachineFreeLook:</dd>
    <dd>-Set Follow attribute to follow the Armature of the "Male_Prototype"</dd>
    <dd>-Set Look At attribute to look at Head of the "Male_Prototype"</dd>
</dl>
