import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { CheckCircle2, XCircle, RotateCcw, ArrowRight, Trophy } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import Header from "@/components/Header";
import { Link } from "react-router-dom";

interface Question {
  id: number;
  question: string;
  options: string[];
  correct: number;
}

const sampleQuestions: Question[] = [
  { id: 1, question: "What is the powerhouse of the cell?", options: ["Nucleus", "Mitochondria", "Ribosome", "Golgi Apparatus"], correct: 1 },
  { id: 2, question: "Which phase of mitosis involves chromosome alignment?", options: ["Prophase", "Metaphase", "Anaphase", "Telophase"], correct: 1 },
  { id: 3, question: "DNA replication occurs in which phase?", options: ["G1 Phase", "S Phase", "G2 Phase", "M Phase"], correct: 1 },
  { id: 4, question: "What structure holds sister chromatids together?", options: ["Centriole", "Spindle fiber", "Centromere", "Telomere"], correct: 2 },
  { id: 5, question: "Which organelle is responsible for protein synthesis?", options: ["Lysosome", "Ribosome", "Vacuole", "Peroxisome"], correct: 1 },
];

const QuizPage = () => {
  const [current, setCurrent] = useState(0);
  const [answers, setAnswers] = useState<Record<number, number>>({});
  const [submitted, setSubmitted] = useState(false);

  const total = sampleQuestions.length;
  const q = sampleQuestions[current];

  const selectAnswer = (optionIndex: number) => {
    if (submitted) return;
    setAnswers((prev) => ({ ...prev, [current]: optionIndex }));
  };

  const next = () => {
    if (current < total - 1) setCurrent((c) => c + 1);
  };

  const handleSubmit = () => setSubmitted(true);

  const score = submitted
    ? sampleQuestions.filter((q, i) => answers[i] === q.correct).length
    : 0;

  const allAnswered = Object.keys(answers).length === total;

  if (submitted) {
    const pct = Math.round((score / total) * 100);
    return (
      <div className="min-h-screen">
        <Header />
        <main className="container max-w-xl py-16">
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="flex flex-col items-center text-center"
          >
            <div className="mb-6 flex h-20 w-20 items-center justify-center rounded-2xl bg-gradient-to-br from-primary to-accent glow-primary">
              <Trophy className="h-10 w-10 text-primary-foreground" />
            </div>
            <h1 className="font-display text-3xl font-bold">Quiz Complete!</h1>
            <p className="mt-2 text-muted-foreground">You scored</p>
            <p className="mt-1 font-display text-6xl font-bold gradient-text">{pct}%</p>
            <p className="mt-2 text-muted-foreground">{score} out of {total} correct</p>

            <div className="mt-8 w-full space-y-3">
              {sampleQuestions.map((q, i) => {
                const correct = answers[i] === q.correct;
                return (
                  <div
                    key={q.id}
                    className={cn(
                      "flex items-center gap-3 rounded-xl border p-4 text-left text-sm",
                      correct ? "border-success/30 bg-success/5" : "border-destructive/30 bg-destructive/5"
                    )}
                  >
                    {correct ? (
                      <CheckCircle2 className="h-5 w-5 shrink-0 text-success" />
                    ) : (
                      <XCircle className="h-5 w-5 shrink-0 text-destructive" />
                    )}
                    <div>
                      <p className="font-medium">{q.question}</p>
                      {!correct && (
                        <p className="mt-0.5 text-xs text-muted-foreground">
                          Correct: {q.options[q.correct]}
                        </p>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>

            <div className="mt-8 flex gap-3">
              <Button variant="outline" asChild>
                <Link to="/">
                  <RotateCcw className="h-4 w-4" /> Back to Dashboard
                </Link>
              </Button>
              <Button variant="glow" asChild>
                <Link to="/upload">Create Another Quiz</Link>
              </Button>
            </div>
          </motion.div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      <Header />
      <main className="container max-w-xl py-12">
        {/* Progress */}
        <div className="mb-8">
          <div className="mb-2 flex items-center justify-between text-sm text-muted-foreground">
            <span>Question {current + 1} of {total}</span>
            <span>{Object.keys(answers).length} answered</span>
          </div>
          <div className="h-1.5 w-full overflow-hidden rounded-full bg-secondary">
            <motion.div
              className="h-full rounded-full bg-gradient-to-r from-primary to-accent"
              initial={{ width: 0 }}
              animate={{ width: `${((current + 1) / total) * 100}%` }}
              transition={{ duration: 0.3 }}
            />
          </div>
        </div>

        <AnimatePresence mode="wait">
          <motion.div
            key={current}
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            exit={{ opacity: 0, x: -20 }}
            transition={{ duration: 0.25 }}
          >
            <h2 className="mb-6 font-display text-xl font-semibold">{q.question}</h2>
            <div className="flex flex-col gap-3">
              {q.options.map((option, i) => (
                <button
                  key={i}
                  onClick={() => selectAnswer(i)}
                  className={cn(
                    "rounded-xl border p-4 text-left text-sm font-medium transition-all",
                    answers[current] === i
                      ? "border-primary bg-primary/10 text-primary"
                      : "border-border bg-card text-foreground hover:border-muted-foreground/50"
                  )}
                >
                  <span className="mr-3 inline-flex h-6 w-6 items-center justify-center rounded-md bg-secondary text-xs font-semibold text-muted-foreground">
                    {String.fromCharCode(65 + i)}
                  </span>
                  {option}
                </button>
              ))}
            </div>
          </motion.div>
        </AnimatePresence>

        <div className="mt-8 flex justify-end gap-3">
          {current < total - 1 ? (
            <Button onClick={next} disabled={answers[current] === undefined}>
              Next <ArrowRight className="h-4 w-4" />
            </Button>
          ) : (
            <Button variant="glow" onClick={handleSubmit} disabled={!allAnswered}>
              Submit Quiz
            </Button>
          )}
        </div>
      </main>
    </div>
  );
};

export default QuizPage;
