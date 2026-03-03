import { useState, useCallback } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Upload, Image, ArrowRight, Zap, FileText, Sparkles, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import Header from "@/components/Header";
import { cn } from "@/lib/utils";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { uploadImageAndGenerateQuiz } from "@/api/quizApi";
import { Difficulty } from "@/api/quizApi";

const difficulties = Object.values(Difficulty);

const UploadPage = () => {
  const [dragActive, setDragActive] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [difficulty, setDifficulty] = useState<Difficulty>(Difficulty.Medium);
  const navigate = useNavigate();

  const handleFile = useCallback((f: File) => {
    setFile(f);
    const reader = new FileReader();
    reader.onload = (e) => setPreview(e.target?.result as string);
    reader.readAsDataURL(f);
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setDragActive(false);
    const f = e.dataTransfer.files[0];
    if (f && (f.type === "image/jpeg" || f.type === "image/png")) handleFile(f);
  }, [handleFile]);

    const mutation = useMutation({
          mutationFn: async () => {
            if (!file) throw new Error("No file uploaded");
            return uploadImageAndGenerateQuiz(file, difficulty);
          },
          onSuccess: (data) => {
            navigate(`/quiz/${data.id}`);
          },
          onError: (error) => {
            console.error(error);
          },
        });
  

  return (
    <div className="min-h-screen">
      <Header />
      <main className="container max-w-2xl py-12">
        <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} className="mb-8 text-center">
          <h1 className="font-display text-3xl font-bold tracking-tight">Create New Quiz</h1>
          <p className="mt-2 text-muted-foreground">Upload your notes and let AI do the rest.</p>
        </motion.div>

        {/* Flow visual */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.2 }}
          className="mb-10 flex items-center justify-center gap-3 text-sm text-muted-foreground"
        >
          <span className="flex items-center gap-1.5 rounded-full bg-secondary px-3 py-1.5">
            <Image className="h-3.5 w-3.5 text-primary" /> Upload
          </span>
          <ArrowRight className="h-3.5 w-3.5" />
          <span className="flex items-center gap-1.5 rounded-full bg-secondary px-3 py-1.5">
            <Sparkles className="h-3.5 w-3.5 text-accent" /> AI Processes
          </span>
          <ArrowRight className="h-3.5 w-3.5" />
          <span className="flex items-center gap-1.5 rounded-full bg-secondary px-3 py-1.5">
            <FileText className="h-3.5 w-3.5 text-success" /> Quiz Ready
          </span>
        </motion.div>

        <AnimatePresence mode="wait">
          {mutation.isPending ? (
            <motion.div
              key="loading"
              initial={{ opacity: 0, scale: 0.95 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0 }}
              className="flex flex-col items-center justify-center py-20"
            >
              <div className="relative mb-6">
                <div className="h-16 w-16 rounded-2xl bg-gradient-to-br from-primary to-accent animate-pulse-glow" />
                <Loader2 className="absolute inset-0 m-auto h-8 w-8 text-primary-foreground animate-spin" />
              </div>
              <h3 className="font-display text-lg font-semibold">Generating your quiz…</h3>
              <p className="mt-1 text-sm text-muted-foreground">AI is analyzing your notes</p>
            </motion.div>
          ) : (
            <motion.div key="form" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}>
              {/* Upload area */}
              <div
                onDragOver={(e) => { e.preventDefault(); setDragActive(true); }}
                onDragLeave={() => setDragActive(false)}
                onDrop={handleDrop}
                className={cn(
                  "relative rounded-2xl border-2 border-dashed transition-all duration-200",
                  dragActive
                    ? "border-primary bg-primary/5 glow-primary"
                    : file
                      ? "border-primary/30 bg-card"
                      : "border-border bg-card/50 hover:border-muted-foreground/30"
                )}
              >
                {preview ? (
                  <div className="flex flex-col items-center gap-4 p-8">
                    <img src={preview} alt="Preview" className="max-h-48 rounded-lg object-contain" />
                    <p className="text-sm text-muted-foreground">{file?.name}</p>
                    <Button variant="ghost" size="sm" onClick={() => { setFile(null); setPreview(null); }}>
                      Remove & re-upload
                    </Button>
                  </div>
                ) : (
                  <label className="flex cursor-pointer flex-col items-center gap-4 p-12">
                    <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-secondary">
                      <Upload className="h-7 w-7 text-muted-foreground" />
                    </div>
                    <div className="text-center">
                      <p className="font-display font-semibold">Drag & drop your notes here</p>
                      <p className="mt-1 text-sm text-muted-foreground">or click to browse · JPG, PNG</p>
                    </div>
                    <input
                      type="file"
                      accept="image/jpeg,image/png"
                      className="hidden"
                      onChange={(e) => e.target.files?.[0] && handleFile(e.target.files[0])}
                    />
                  </label>
                )}
              </div>

              {/* Difficulty selector */}
              <div className="mt-6">
                <label className="mb-2 block text-sm font-medium text-muted-foreground">Difficulty</label>
                <div className="flex gap-2">
                  {difficulties.map((d) => (
                    <button
                      key={d}
                      onClick={() => setDifficulty(d)}
                      className={cn(
                        "flex-1 rounded-xl border py-2.5 text-sm font-medium transition-all",
                        difficulty === d
                          ? "border-primary bg-primary/10 text-primary"
                          : "border-border bg-card text-muted-foreground hover:text-foreground"
                      )}
                    >
                      {d}
                    </button>
                  ))}
                </div>
              </div>

              {/* Generate button */}
              <Button
                variant="glow"
                size="lg"
                className="mt-8 w-full text-base"
                disabled={!file || mutation.isPending}
                onClick={() => mutation.mutate()}
              >
                  {mutation.isPending ? (
                    <>
                      <Loader2 className="h-4 w-4 animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Zap className="h-4 w-4" />
                      Generate Quiz
                    </>
                  )}
              </Button>
            </motion.div>
          )}
        </AnimatePresence>
      </main>
    </div>
  );
};

export default UploadPage;
