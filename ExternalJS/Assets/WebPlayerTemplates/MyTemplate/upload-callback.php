<?php
	include ("upload.php")
?>
<script>self.parent.frames['WebPlayerFrame'].GetUnity ().SendMessage ('Control', 'OnUploaded', document.body.innerHTML);</script>
