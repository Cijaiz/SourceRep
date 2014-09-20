using BE = C2C.BusinessEntities.C2CEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on Quiz Entity Libraries.
    /// </summary>
    public class QuizWorker
    {
        /// <summary>
        /// Creates a new instance for QuizWorker class
        /// </summary>
        /// <returns>Returns the new instance</returns>
        public static QuizWorker GetInstance()
        {
            return new QuizWorker();
        }

        public BE.Quiz GetQuiz(int id)
        {
            return null;
        }

        public List<BE.Quiz> GetQuizList(Pager pager, Status status, string title)
        {
            return null;
        }

        public ProcessResponse<BE.Quiz> CreateQuiz(BE.Quiz quiz)
        {
            return null;
        }

        public ProcessResponse UpdateQuiz(BE.Quiz quiz)
        {
            return null;
        }

        public ProcessResponse DeleteQuiz(int quizId)
        {
            return null;
        }

        public ProcessResponse PublishQuiz(int quizId)
        {
            return null;
        }

        public ProcessResponse UnPublishQuiz(int quizId)
        {
            return null;
        }

        public int GetQuizCount(Status status)
        {
            return 0;
        }

        public BE.QuizParticipant GetParticipants(int quizId)
        {
            return null;
        }

        public int GetParticipantsCount(int quizId)
        {
            return 0;
        }

        public ProcessResponse UpdateParticipant(BE.QuizParticipant participant)
        {
            return null;
        }

        public ProcessResponse AddParticipant(BE.QuizParticipant participant)
        {
            return null;
        }

        public ProcessResponse SubmitQuiz(int quizId, int userId)
        {
            return null;
        }

        public BE.Question CreateQuestion(BE.Question question)
        {
            return null;
        }

        public ProcessResponse UpdateQuestion(BE.Question question)
        {
            return null;
        }

        public ProcessResponse Delete(int questionId)
        {
            return null;
        }

        public BE.Question GetQuestion(int questionId)
        {
            return null;
        }

        public List<BE.Question> GetQuestionList(Pager pager, Status status)
        {
            return null;
        }

        public int GetQuestionCount(Status status)
        {
            return 0;
        }

        public BE.QuizParticipantsAnswer SubmitParticipantAnswer(BE.QuizParticipantsAnswer answer)
        {
            return null;
        }

        public ProcessResponse AddQuizQuestions(int quizId, List<int> questionId)
        {
            return null;
        }

    }

}
