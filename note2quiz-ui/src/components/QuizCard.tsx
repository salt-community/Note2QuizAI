import { motion } from "framer-motion";
import { Clock, BarChart3, ChevronRight } from "lucide-react";
import { Link } from "react-router-dom";
import { cn } from "@/lib/utils";
import { Difficulty } from "@/api/quizApi";

interface QuizCardProps {
  id: string;
  title: string;
  date: string;
  difficulty:Difficulty
  score?: number;
  questionCount: number;
  index?: number;
}

const difficultyColor = {
  Easy: "text-success bg-success/10",
  Medium: "text-warning bg-warning/10",
  Hard: "text-destructive bg-destructive/10",
};

const QuizCard = ({ id, title, date, difficulty, score, questionCount, index = 0 }: QuizCardProps) => (
  <motion.div
    initial={{ opacity: 0, y: 12 }}
    animate={{ opacity: 1, y: 0 }}
    transition={{ duration: 0.3, delay: index * 0.05 }}
  >
    <Link
      to={`/quiz/${id}`}
      className="group flex items-center justify-between rounded-xl border border-border/50 bg-card p-4 transition-all hover:border-primary/30 hover:bg-card/80"
    >
      <div className="flex flex-col gap-1.5">
        <h3 className="font-display font-semibold group-hover:text-primary transition-colors">
          {title}
        </h3>
        <div className="flex items-center gap-3 text-sm text-muted-foreground">
          <span className="flex items-center gap-1">
            <Clock className="h-3.5 w-3.5" /> {date}
          </span>
          <span className="flex items-center gap-1">
            <BarChart3 className="h-3.5 w-3.5" /> {questionCount} questions
          </span>
          <span className={cn("rounded-md px-2 py-0.5 text-xs font-medium", difficultyColor[difficulty])}>
            {difficulty}
          </span>
        </div>
      </div>
      <div className="flex items-center gap-3">
        {score !== undefined && score != null && (
          <span className="font-display text-lg font-bold text-primary">{score}%</span>
        )}
        <ChevronRight className="h-4 w-4 text-muted-foreground transition-transform group-hover:translate-x-1" />
      </div>
    </Link>
  </motion.div>
);

export default QuizCard;
