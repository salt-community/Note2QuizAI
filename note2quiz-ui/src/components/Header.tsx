import { Zap, FileText, User } from "lucide-react";
import { Link, useLocation } from "react-router-dom";
import { cn } from "@/lib/utils";

const Header = () => {
  const location = useLocation();

  return (
    <header className="sticky top-0 z-50 glass border-b border-border/50">
      <div className="container flex h-16 items-center justify-between">
        <Link to="/" className="flex items-center gap-2.5">
          <div className="relative flex h-9 w-9 items-center justify-center rounded-lg bg-gradient-to-br from-primary to-accent">
            <FileText className="h-4 w-4 text-primary-foreground" />
            <Zap className="absolute -right-1 -top-1 h-3.5 w-3.5 text-warning" />
          </div>
          <span className="font-display text-xl font-bold tracking-tight">
            Note<span className="gradient-text">2Quiz</span>
          </span>
        </Link>

        <nav className="flex items-center gap-1">
          <Link
            to="/"
            className={cn(
              "rounded-lg px-3.5 py-2 text-sm font-medium transition-colors",
              location.pathname === "/"
                ? "bg-secondary text-foreground"
                : "text-muted-foreground hover:text-foreground"
            )}
          >
            Dashboard
          </Link>
          <Link
            to="/upload"
            className={cn(
              "rounded-lg px-3.5 py-2 text-sm font-medium transition-colors",
              location.pathname === "/upload"
                ? "bg-secondary text-foreground"
                : "text-muted-foreground hover:text-foreground"
            )}
          >
            Create Quiz
          </Link>
        </nav>

        <button className="flex h-9 w-9 items-center justify-center rounded-full bg-secondary text-muted-foreground transition-colors hover:text-foreground">
          <User className="h-4 w-4" />
        </button>
      </div>
    </header>
  );
};

export default Header;
