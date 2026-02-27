import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Index from "./pages/Index";
import UploadPage from "./pages/UploadPage";
import QuizPage from "./pages/QuizPage";
import NotFound from "./pages/NotFound";
import { RedirectToSignIn, SignedIn, SignedOut } from "@clerk/clerk-react";

const queryClient = new QueryClient();

const App = () => (
  <>
   <SignedOut>
      <RedirectToSignIn />
    </SignedOut>

    <SignedIn>
      <QueryClientProvider client={queryClient}>
        <TooltipProvider>
          <Toaster />
          <Sonner />
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Index />} />
              <Route path="/upload" element={<UploadPage />} />
              <Route path="/quiz/:id" element={<QuizPage />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </BrowserRouter>
        </TooltipProvider>
      </QueryClientProvider>
    </SignedIn>
    </>
);

export default App;
