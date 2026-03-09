import { render, screen } from "@testing-library/react";
import QuizCard from "@/components/QuizCard";
import { Difficulty } from "@/api/quizApi";
import { MemoryRouter } from "react-router-dom";

test("renders QuizCard with title and difficulty", () => {
	render(
		<MemoryRouter>
			<QuizCard id="1" title="Sample Quiz" date="2026-03-09" difficulty={Difficulty.Easy} score={80} questionCount={5} />
		</MemoryRouter>
	);

	expect(screen.getByText("Sample Quiz")).toBeInTheDocument();
	expect(screen.getByText("Easy")).toBeInTheDocument();
	expect(screen.getByText("80%")).toBeInTheDocument();
});
