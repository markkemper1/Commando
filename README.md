Commando
========

A helper library for creating command based applications

Command executor is responsible for providing dependencies to commands.

It allows commands to be nested and aggregated without concerning the command logic with dependencies.

The CommandExecutor has a list of "BeforeExecute" actions that allow services or dependencies to be provided to commands.

A quickly typed (will have some typo's) example is shown below, Register a user.

	public class RegisterUserCommand : CommandResultBase<int>
	{
		public RegisterUserCommand(RegistrationForm registrationForm)
		{
			this.RegistrationForm = registrationForm;
		}
		
		public RegistrationForm Registration {get;set;}

		//ICommand interface
		pubic void Execute()
		{
			var userId = this.Execute(new InsertUserCommand(this.Registration);

			var activationToken = this.Execute(new ResetUserPassword(userId));

			this.Execute(new SendRegistrationEmailCommand(this.Registration, activationToken);

			return userId;
		}
	}


	public class SendRegistrationEmailCommand : EmailCommandBase
	{
		public SendRegistrationEmailCommand(RegistrationForm registrationForm, string token)
		{
			this.RegistrationForm = registrationForm;
			this.Token = token;
		}
		
		public string Token {get;set}
		public RegistrationForm Registration {get;set;}

		public override MailMessage CreateMailMessage()
		{
			/* code to create mail message */
			return new MailMessage(); 
		}
	}

	public class EmailCommandBase : ICommand
	{
		public override void Execute()
		{
			var m = this.CreateMailMessage();
			
			Smtp.Send(m);
			 or
			DbInsert(m)
			 or
			MyServiceProxy.Send(m);
		}

		public abstract MailMessage CreateMailMessage()
	}

	public class InsertUserCommand : DbCommandBase<int>
	{
		public RegisterUserCommand(RegistrationForm registrationForm)
		{
			this.RegistrationForm = registrationForm;
		}
		
		public RegistrationForm Registration {get;set;}

		public override int Execute(IDbConnection db)
		{
			return /* sql to insert user */
		}
	}