import { motion } from "framer-motion";
import { Plus, BookOpen, Flame, Trophy } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";
import Header from "@/components/Header";
import StatsCard from "@/components/StatsCard";
import QuizCard from "@/components/QuizCard";
import EmptyState from "@/components/EmptyState";

const sampleQuizzes = [
  { id: "1", title: "Biology Chapter 5: Cell Division", date: "Feb 25, 2026", difficulty: "Medium" as const, score: 85, questionCount: 10 },
  { id: "2", title: "History: World War II Key Events", date: "Feb 24, 2026", difficulty: "Hard" as const, score: 72, questionCount: 15 },
  { id: "3", title: "Math: Linear Algebra Basics", date: "Feb 23, 2026", difficulty: "Easy" as const, score: 95, questionCount: 8 },
  { id: "4", title: "Chemistry: Organic Reactions", date: "Feb 22, 2026", difficulty: "Hard" as const, questionCount: 12 },
];

const Index = () => {
  const hasQuizzes = sampleQuizzes.length > 0;

  return (
    <div className="min-h-screen">
      <Header />
      <main className="container py-8">
        <motion.div
  initial={{ opacity: 0, y: -10 }}
  animate={{ opacity: 1, y: 0 }}
  className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between"
>
  <div>
    <h1 className="font-display text-3xl font-bold tracking-tight">Dashboard</h1>
    <p className="mt-1 text-muted-foreground">Welcome back! Here's your quiz overview.</p>
  </div>

  <Button variant="glow" size="lg" asChild className="w-full sm:w-auto">
    <Link to="/upload" className="justify-center">
      <Plus className="h-4 w-4" />
      Create New Quiz
    </Link>
  </Button>
</motion.div>

        {hasQuizzes && (
          <>
            <div className="mb-8 grid grid-cols-1 gap-4 sm:grid-cols-3">
              <StatsCard icon={BookOpen} label="Quizzes Created" value={12} delay={0} />
              <StatsCard icon={Flame} label="Day Streak" value={5} delay={0.1} />
              <StatsCard icon={Trophy} label="Avg. Score" value="84%" delay={0.2} />
            </div>

            <div className="mb-4">
              <h2 className="font-display text-lg font-semibold">Quiz History</h2>
            </div>
            <div className="flex flex-col gap-3">
              {sampleQuizzes.map((quiz, i) => (
                <QuizCard key={quiz.id} {...quiz} index={i} />
              ))}
            </div>
          </>
        )}

        {!hasQuizzes && <EmptyState />}
      </main>
    </div>
  );
};

export default Index;
