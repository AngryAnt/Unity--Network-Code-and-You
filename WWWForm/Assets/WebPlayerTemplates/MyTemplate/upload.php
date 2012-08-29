<?php
	$root = "/users/angryant/MAMP/";
	$rootURL = "localhost";
	$filename = $_FILES['file']['name'];

	if (move_uploaded_file ($_FILES['file']['tmp_name'], $root . $filename))
	{
		echo ("http://" . $rootURL . "/" . $filename);
	}
	else
	{
		echo ("File upload failed or no file provided");
	}
?>
