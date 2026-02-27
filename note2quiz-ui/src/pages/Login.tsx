import { SignIn } from "@clerk/clerk-react";

const Login = () => {
  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-6">
      <SignIn
        routing="hash"
        afterSignInUrl="/"
        appearance={{
          elements: {
            headerTitle: "text-white",
      headerSubtitle: "text-muted-foreground",
            rootBox: "mx-auto",
            card: "bg-card border border-border shadow-xl",
            socialButtonsBlockButton: "text-white",
      socialButtonsBlockButtonText: "text-white",
dividerText: "text-gray-400",
      formFieldLabel: "text-white",
      formFieldInput: "text-white",
          },
        }}
      />
   
    </div>
  );
};

export default Login;
