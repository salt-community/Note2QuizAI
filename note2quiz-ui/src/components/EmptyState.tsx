import { motion } from "framer-motion";
import { FileQuestion } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";

const EmptyState = () => (
  <motion.div
    initial={{ opacity: 0, scale: 0.95 }}
    animate={{ opacity: 1, scale: 1 }}
    transition={{ duration: 0.5 }}
    className="flex flex-col items-center justify-center py-20 text-center"
  >
    <div className="mb-6 flex h-20 w-20 items-center justify-center rounded-2xl bg-secondary">
      <FileQuestion className="h-10 w-10 text-muted-foreground" />
    </div>
    <h3 className="mb-2 font-display text-xl font-semibold">No quizzes yet</h3>
    <p className="mb-6 max-w-sm text-muted-foreground">
      Upload your notes and let AI generate quiz questions for you. It only takes a few seconds.
    </p>
    <Button variant="glow" size="lg" asChild>
      <Link to="/upload">Create Your First Quiz</Link>
    </Button>
  </motion.div>
);

export default EmptyState;
