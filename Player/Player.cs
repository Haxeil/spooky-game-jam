using Godot;
using System;

public class Player : KinematicBody2D
{
	[Export] private Vector2 velocity = Vector2.Zero;
	[Export] private float speed = 50.0f;
	[Export] private float friction = 0.1f;
	[Export] private float jumpPower = 200;

	[Export] private float acceleration = 0.1f;
	[Export] private float gravity = 20;
	private int direction = 1;
	[Export] private float wallSlideSpeedDown = 80.0f;
	[Export] private float wallSlideSpeedUp = 90.0f;
	[Export] private bool boardingUp = false;
	[Export] private float maxWallSlideSpeed = 320.0f;
	[Export] private bool isWallSliding = false;
	private AnimatedSprite animSprite;
	private bool wasSliding = false;
	private Timer cayoteWallJump;
	private Node2D inFloor;
	private Node2D inCelling;
	private Node2D rightCheckSlide;
	private Node2D leftCheckSlide;
	private Timer cayoteTimer;
	private Timer bloodClot;
	private bool isGrounded = false;
	[Export] public float maxHealth = 100;
	[Export] public float health;
	[Export] public float maxEnergy = 100;
	[Export] public float energy;
	[Signal] public delegate void changeState(StateMachine state);
	[Signal] public delegate void healthUpdated(float health);
	[Signal] public delegate void maxHealthUpdated(float maxHealth);

	[Signal] public delegate void energyUpdated(float energy);
	[Signal] public delegate void maxEnergyUpdated(float maxEnergy);
	[Signal] public delegate void killed(Player player);

	StateMachine state = new StateMachine();
	public override void _Ready() {
		Init();
		RayException();
		this.health = this.maxHealth;
		this.energy = this.maxEnergy;
		EmitSignal("maxEnergyUpdated", maxEnergy);
		EmitSignal("maxHealthUpdated", maxHealth);
		
		//EmitSignal("healthUpdated", this.health);
		EmitSignal("changeState", Enums.PlayerState.IDLE);
	}

	private void Init() {
		cayoteWallJump = GetNode<Timer>("CayoteWallJump");
		animSprite = GetNode<AnimatedSprite>("AnimSprite");
		inFloor = GetNode<Node2D>("InFloor");
		inCelling = GetNode<Node2D>("InCelling");
		rightCheckSlide = GetNode<Node2D>("RightCheckSlide");
		leftCheckSlide = GetNode<Node2D>("LeftCheckSlide");
		cayoteTimer = GetNode<Timer>("CayoteTime");
		bloodClot = GetNode<Timer>("BloodClot");
	}
	public override void _PhysicsProcess(float delta)
	{
		AppyGravity();
		Jump();
		Move(delta);	

	}


	private void Move(float delta) {
		
		if (this.state.currentState != Enums.PlayerState.HURT) {
			if (Input.IsActionPressed("Right") && !Input.IsActionPressed("Left") && NotDeadOrHurt() ) {
				if (!this.isWallSliding && this.isGrounded) {
					EmitSignal("changeState", Enums.PlayerState.RUN);
				}
				direction = 1;
				velocity.x = Mathf.Lerp(velocity.x, speed * direction, this.acceleration);
			} else if (Input.IsActionPressed("Left") && !Input.IsActionPressed("Right") && NotDeadOrHurt() ) {

				if (!this.isWallSliding && this.isGrounded) {
					EmitSignal("changeState", Enums.PlayerState.RUN);

				}
				direction = -1;
				velocity.x = Mathf.Lerp(velocity.x, speed * direction, this.acceleration);

			} else {
				if (this.isGrounded && NotDeadOrHurt()) {
					EmitSignal("changeState", Enums.PlayerState.IDLE);
				}
				velocity.x = Mathf.Lerp(velocity.x, 0, this.friction);
			}
		}


		FlipH(this.direction);

		WallSlide(delta);

		bool wasOnFloor = this.isGrounded;

		MoveAndSlide(velocity, Vector2.Up, false, 4, -Mathf.Pi/4, false);

		this.isGrounded = RayCheckOnFloor();

		if (!this.isGrounded && wasOnFloor)
		{
			cayoteTimer.Start();
		}

		
	}

	private void FlipH(int direction) {

		if (direction == 1) {
			this.animSprite.FlipH = false;
		} else {
			this.animSprite.FlipH = true;
		}

	}
	private void AppyGravity() {

		if (RayCheckOnCelling()) {
			velocity.y = 0;
			velocity.y = gravity;
		}


		if (this.isGrounded) {
			if (NotDeadOrHurt()) {
				velocity.y = 0;

			}
		} else {
			velocity.y += this.gravity;

			if (this.velocity.y != 0 && this.velocity.y > this.gravity && this.velocity.y > -this.jumpPower/this.gravity)
			{
				EmitSignal("changeState", Enums.PlayerState.FALL);
			}
		}
		
	}

	private void Jump() {


		if (Input.IsActionPressed("Jump")) {
			if (RayCheckOnFloor() || !this.cayoteTimer.IsStopped()) {
				//ApplyDamage(25);
				cayoteTimer.Stop();
				velocity.y = -jumpPower;

				EmitSignal("changeState", Enums.PlayerState.JUMP);
				

			}

		}

	}

	private void OnChangeState(Enums.PlayerState state)
	{

		if (this.state.currentState == Enums.PlayerState.DEAD) {
			return;
		}
		// if (state == Enums.PlayerState.DEAD || this.state.currentState == Enums.PlayerState.DEAD) {
		// 	this.state.currentState = Enums.PlayerState.DEAD;
		// 	return;
		// }

		this.state.nexState = state; //Jump // falling // moving // moving 
		

		
		// if (this.state.nexState != Enums.PlayerState.SLIDE) {
		// 	if ((this.state.currentState == Enums.PlayerState.JUMP || this.state.currentState == Enums.PlayerState.FALL) && !this.isGrounded) {
		// 		return;
		// 	}
		// }

		


		// dont change the prevState if the nextState = the currentState
		if (this.state.nexState != this.state.currentState)
		{
			this.state.pervState = this.state.currentState; // idle // jump //fall //moving
		}
		this.state.currentState = this.state.nexState; //jump  // fall // moving // moving
		PlayAnimation();
	}


	private void PlayAnimation() {
		
		switch (this.state.currentState) {
			case Enums.PlayerState.IDLE:
				this.animSprite.Play("Idle");
				break;
			case Enums.PlayerState.RUN:
				this.animSprite.Play("Run");
				break;
			case Enums.PlayerState.BOARD:
				this.animSprite.Play("Board");
				break;
			case Enums.PlayerState.JUMP:
				this.animSprite.Play("Jump");
				break;
			case Enums.PlayerState.SLIDE:
				this.animSprite.Play("Slide");
				break;
			case Enums.PlayerState.FALL:
				this.animSprite.Play("Fall");
				break;
			case Enums.PlayerState.HURT:
				this.animSprite.Play("Hurt");
				break;
			case Enums.PlayerState.DEAD: 
				this.animSprite.Play("Death");
				break;

		}

	}

	private void WallSlide(float delta) {

		if (RayCheckWallSliding() && !this.isGrounded && !RayCheckOnCelling()) {
			this.isWallSliding = true;
			this.wasSliding = true;
		} else {
			this.isWallSliding = false;
		}
		// Using a Raycast is better to check if Player IsOnFloor();
		if (this.isWallSliding && !this.isGrounded && !RayCheckOnCelling()) {
			if (Input.IsActionPressed("Down")) {
				velocity.y += this.wallSlideSpeedDown * delta;
				velocity.y = Mathf.Clamp(velocity.y, this.wallSlideSpeedDown, this.maxWallSlideSpeed);
			} else {
				this.velocity.y = 0;
			}
			if (this.direction == 1) {
				animSprite.SetDeferred("flip_h", true);
			} else {
				animSprite.SetDeferred("flip_h", false);
			}
			if (!this.boardingUp) {
				EmitSignal("changeState", Enums.PlayerState.SLIDE);

			}
		}

		if (!RayCheckOnCelling() && !this.isGrounded && this.wasSliding && !this.isWallSliding) {
			this.cayoteWallJump.Start();
			wasSliding = false;
		}

		if (RayCheckOnCelling()) {
			
			velocity.y = this.gravity + (velocity.y);
			return;
		}
		
		if (Input.IsActionPressed("Jump") && this.isWallSliding) {
			// Wall Slide Jump;
			this.boardingUp = true;
			this.velocity.y = - wallSlideSpeedUp;
			EmitSignal("changeState", Enums.PlayerState.BOARD);

		} else if (Input.IsActionPressed("Jump") && !this.cayoteWallJump.IsStopped()) {
			// Wall Slide Jump;
			this.velocity.y = - jumpPower;
			EmitSignal("changeState", Enums.PlayerState.JUMP);		
		} else {
			this.boardingUp = false;
		}
		



	}
	private void RayException() {
		foreach (RayCast2D ray in this.leftCheckSlide.GetChildren()) {
			ray.AddException(this);
		}
		foreach (RayCast2D ray in this.rightCheckSlide.GetChildren()) {
			ray.AddException(this);
		}
		foreach (RayCast2D ray in this.inCelling.GetChildren()) {
			ray.AddException(this);
		}
		foreach (RayCast2D ray in this.inFloor.GetChildren()) {
			ray.AddException(this);
		}
	}

	//checking if player grounded by RayCasting rather than the Default IsOnfloor()
	private bool RayCheckOnFloor() {
		foreach (RayCast2D ray in this.inFloor.GetChildren()) {
			if (ray.IsColliding()) {
				return true;
			} else {
				continue;
			}
		}
		return false;

	}

	private bool RayCheckOnCelling() {
		
		foreach (RayCast2D ray in this.inCelling.GetChildren()) {
			if (ray.IsColliding()) {
				return true;
			} else {
				return false;
			}
		}

		return false;

	}



	private bool RayCheckWallSliding() {
		bool rightSide = false;
		bool leftSide = false;

		foreach (RayCast2D ray in this.leftCheckSlide.GetChildren()) {
			if (ray.IsColliding()) {
				leftSide = true;
			} else {
				leftSide = false;
			}
		}

		foreach (RayCast2D ray in this.rightCheckSlide.GetChildren()) {
			if (ray.IsColliding()) {
				rightSide = true;
			} else {
				rightSide = false;
			}
		}
		return leftSide || rightSide;
	}
	public void SetEnergy(float value) {
		var prevEnergy = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.maxEnergy);
		if (prevEnergy != this.energy)
		{	
			EmitSignal("energyUpdated", this.energy);
			if (this.energy == 0)
			{
				this.bloodClot.Start();
			}
		}
	}
	public void SetHealth(float value) {
		var prevHealth = this.health;
		this.health = Mathf.Clamp(value, 0, this.maxHealth);
		if (prevHealth != this.health)
		{	
			EmitSignal("healthUpdated", this.health);
			if (this.health == 0)
			{
				Die();
				EmitSignal("killed", this);
			}
		}
	}
	private void ApplyDamage(float amount, Vector2 damageVector, int enemyDirection) {
		EmitSignal("changeState", Enums.PlayerState.HURT);
		SetHealth(this.health - amount);
		this.velocity = Vector2.Zero;
		this.velocity += new Vector2(damageVector.x * enemyDirection, -damageVector.y);

	}

	private void ApplyEnergyDamage(float amount){
		SetEnergy(this.energy - amount);
	}

	async private void Die() {

		EmitSignal("changeState", Enums.PlayerState.DEAD);
		await ToSignal(this.animSprite, "animation_finished");
		//QueueFree();
	}

	private bool NotDeadOrHurt() {
		return this.state.currentState != Enums.PlayerState.HURT 
		&& this.state.currentState != Enums.PlayerState.DEAD;
	}
	//Called From CheckPoint GDScript
	private void StopBloodClot() {
		this.bloodClot.Stop();
		SetEnergy(this.maxEnergy);
	}
	private void _on_BloodClot_timeout()
	{
		ApplyDamage(2, Vector2.Zero, 1);
	}

	private void _on_BloodClotEveryTime_timeout()
	{
		ApplyEnergyDamage(5);
	}

}
