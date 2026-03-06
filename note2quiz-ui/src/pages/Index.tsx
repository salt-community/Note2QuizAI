import { motion } from "framer-motion";
import { Plus, BookOpen, Flame, Trophy } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";
import Header from "@/components/Header";
import StatsCard from "@/components/StatsCard";
import QuizCard from "@/components/QuizCard";
import EmptyState from "@/components/EmptyState";
import { useAuth } from "@clerk/clerk-react";
import { useQuery } from "@tanstack/react-query";
import { quizHistory } from "@/api/quizApi";


const Index = () => {
  const {getToken} = useAuth();
  const {data:quizzes,isLoading,isError} = useQuery({
    queryKey: ["quizHistory"],
    queryFn:async() =>{
      const token = await getToken();
      if(!token) throw new Error("Token not available");
      return quizHistory(token);
    }
  })
console.log("-------index page quizzes----",quizzes);
const hasQuizzes = quizzes && quizzes.length > 0
const scoredQuizzes = quizzes?.filter(q => q.score !== null) || [];
const avgScore = scoredQuizzes.length > 0
  ? Math.round(scoredQuizzes.reduce((sum, q) => sum + q.score!, 0) / scoredQuizzes.length)
  : 0;

console.log("-------avgScore----",avgScore);

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
              <StatsCard icon={BookOpen} label="Quizzes Created" value={quizzes.length} delay={0} />
              <StatsCard icon={Trophy} label="Avg. Score" value={`${avgScore}%`} delay={0.2} />
            </div>

            <div className="mb-4">
              <h2 className="font-display text-lg font-semibold">Quiz History</h2>
            </div>
            <div className="flex flex-col gap-3">
              {quizzes?.map((quiz, i) => (
                 <QuizCard
                    key={quiz.quizSessionId}
                    id={quiz.quizSessionId.toString()}
                    title={`Quiz ${quiz.quizSessionId}`}
                    date={new Date(quiz.createdAt).toLocaleDateString()}
                    difficulty={quiz.difficulty}
                    score={quiz.score}
                    questionCount={quiz.questionCount}
                    index={i}
                  />
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
