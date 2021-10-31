extends KinematicBody2D


export(Vector2) var velocity = Vector2()
export(float) var speed = 0
var can_move := false;
var direction := 1;
export(float) var damage = 25;
export(Vector2) var vector_damage = Vector2()



func _physics_process(delta):
	flip_direction()
	move()

func move():
	if can_move:
		$Anim.play("Move")
		velocity.x = speed * direction
		move_and_slide(velocity, Vector2.UP)

func flip_direction():
	if not $Floor.is_colliding():
		direction *= -1
		scale.x *= -1
	if $Wall.is_colliding():
		direction *= -1
		scale.x *= -1

func _on_Hit_body_entered(body):
	
	if body.has_method("ApplyDamage"):
		body.ApplyDamage(damage, vector_damage, direction)
		print("hit")


func _on_VisibilityNotifier2D_screen_entered():
	can_move = true
	$Anim.play("Move")
	show()


func _on_VisibilityNotifier2D_screen_exited():
	can_move = false
	$Anim.stop();
	hide()
