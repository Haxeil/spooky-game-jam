extends Area2D

var one_time = true;


func _on_CheckPoint_body_entered(body):
	if body is KinematicBody2D and one_time:
		one_time = false;
		$AnimatedSprite.play("A");
		body.StopBloodClot();
		$Shrine.play()



