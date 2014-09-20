#region References
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the Group and Uses GroupWorker to do Data manipulation.
    /// </summary>
    public static class QuizManager
    {
        public static Quiz GetQuiz(int id)
        {
            return null;
        }

        public static List<Quiz> GetQuizList(Pager pager, Status status, string title)
        {
            return null;
        }

        public static ProcessResponse<Quiz> CreateQuiz(Quiz quiz)
        {
            return null;
        }

        public static ProcessResponse UpdateQuiz(Quiz quiz)
        {
            return null;
        }

        public static ProcessResponse DeleteQuiz(int quizId)
        {
            return null;
        }

        public static ProcessResponse PublishQuiz(int quizId)
        {
            return null;
        }

        public static ProcessResponse UnPublishQuiz(int quizId)
        {
            return null;
        }

        public static int GetQuizCount(Status status)
        {
            return 0;
        }

        public static QuizParticipant GetParticipants(int quizId,Pager pager)
        {
            return null;
        }

        public static int GetParticipantsCount(int quizId)
        {
            return 0;
        }

        public static ProcessResponse UpdateParticipant(QuizParticipant participant)
        {
            return null;
        }

        public static ProcessResponse AddParticipant(QuizParticipant participant)
        {
            return null;
        }

        public static ProcessResponse SubmitQuiz(int quizId, int userId)
        {
            return null;
        }

        public static Question CreateQuestion(Question question)
        {
            return null;
        }

        public static ProcessResponse UpdateQuestion(Question question)
        {
            return null;
        }

        public static ProcessResponse Delete(int questionId)
        {
            return null;
        }

        public static Question GetQuestion(int questionId)
        {
            return null;
        }

        public static List<Question> GetQuestionList(Pager pager, Status status)
        {
            return null;
        }

        public static int GetQuestionCount(Status status)
        {
            return 0;
        }

        public static QuizParticipantsAnswer SubmitParticipantAnswer(QuizParticipantsAnswer answer)
        {
            return null; 
        }

        public static ProcessResponse AddQuizQuestions(int quizId, List<int> questionId) 
        {
            return null;
        }
    }
}
