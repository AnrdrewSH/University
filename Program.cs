using System;
using System.Collections.Generic;

namespace university
{
	class Program
	{
		static DBMS dbms = new DBMS("DatabasePath=db.json;");

		static void displayReport()
		{
			UI.showReport(dbms.getObjects<Lesson>().Count, dbms.getObjects<Group>().Count,
				dbms.getObjects<Student>().Count, dbms.getObjects<Instructor>().Count);
		}

		/*
			Lesson.
		*/
		static void addLesson(int iDeletionCandidateIndex = -1)
		{
			Dictionary<string, string> fields;

			if (iDeletionCandidateIndex == -1)
			{
				fields = UI.inputFields(new string[]{
					"name",
					"instructorId"
				});
			}
			else
			{
				Lesson lesson = dbms.getObjects<Lesson>()[iDeletionCandidateIndex];

				int iInstructorIndex = dbms.getIndexById<Instructor>(lesson.instructorId);

				fields = UI.inputFields(new string[]{
					"name",
					"instructorId"
				}, new string[]{
					lesson.name,
					iInstructorIndex > -1 ? (iInstructorIndex + 1).ToString() : null
				});
			}

			/*
				Validate input data.
			*/
			ValidateResult result = Validator.validateLesson(fields, dbms.getObjects<Instructor>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[validate error]: {result.sMessage}..");
				return;
			}

			if (fields["instructorId"].Length > 0)
				fields["instructorId"] = dbms.getObjects<Instructor>()[int.Parse(fields["instructorId"]) - 1].id;

			if (iDeletionCandidateIndex != -1)
				dbms.updateObject<Lesson>(iDeletionCandidateIndex, fields);
			else
				dbms.insertObject<Lesson>(0, fields);
	
			UI.showLessons(dbms.getObjects<Lesson>());
		}

		static void deleteLesson(int iIndex)
		{
			dbms.deleteIdenticalIdsFromGroupsLessonIds(dbms.getObjects<Lesson>()[iIndex].id);
			dbms.deleteByIndex<Lesson>(iIndex);
			UI.showLessons(dbms.getObjects<Lesson>());
		}

		static void editLesson(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Lesson>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Console.WriteLine($"\n[edit lesson | i={iIndex + 1}]:");
			addLesson(iIndex);
		}

		static void displayLesson(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Lesson>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Lesson lesson = dbms.getObjects<Lesson>()[iIndex];

			int iInstructorIndex = -1;
			Instructor instructor = new Instructor();

			if (lesson.instructorId.Length > 0)
			{
				iInstructorIndex = dbms.getIndexById<Instructor>(lesson.instructorId);
				instructor = dbms.getObjects<Instructor>()[iInstructorIndex];
			}

			UI.showLesson(lesson, iIndex, dbms.getGroupsByLessonId(lesson.id), instructor, iInstructorIndex);
		}

		/*
			Group.
		*/
		static void addGroup(int iDeletionCandidateIndex = -1)
		{
			Dictionary<string, string> fields;

			if (iDeletionCandidateIndex == -1)
			{
				fields = UI.inputFields(new string[]{
					"number",
					"lessonsIds"
				});
			}
			else
			{
				Group group = dbms.getObjects<Group>()[iDeletionCandidateIndex];

				string sLessonsIds = "";
				for (int i = 0; i < group.lessonsIds.Count; ++i)
				{
					int iLessonIndex = dbms.getIndexById<Lesson>(group.lessonsIds[i]);
					if (iLessonIndex > -1)
						sLessonsIds += (iLessonIndex + 1).ToString() + (i < group.lessonsIds.Count - 1 ? " " : "");
				}

				fields = UI.inputFields(new string[]{
					"number",
					"lessonsIds"
				}, new string[]{
					group.number,
					sLessonsIds
				});
			}

			/*
				Validate input data.
			*/
			ValidateResult result = Validator.validateGroup(fields, dbms.getObjects<Lesson>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[validate error]: {result.sMessage}..");
				return;
			}

			if (iDeletionCandidateIndex != -1)
				dbms.updateObject<Group>(iDeletionCandidateIndex, fields);
			else
				dbms.insertObject<Group>(0, fields);

			UI.showGroups(dbms.getObjects<Group>());
		}

		static void deleteGroup(int iIndex)
		{
			dbms.deleteIdenticalGroupIdsFromStudents(dbms.getObjects<Group>()[iIndex].id);
			dbms.deleteByIndex<Group>(iIndex);
			UI.showGroups(dbms.getObjects<Group>());
		}

		static void editGroup(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Group>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Console.WriteLine($"\n[edit group | i={iIndex + 1}]:");
			addGroup(iIndex);
		}

		static void displayGroup(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Group>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Group group = dbms.getObjects<Group>()[iIndex];

			UI.showGroup(group, iIndex, dbms.getLessonsByIds(group.lessonsIds), dbms.getStudentsByGroupId(group.id));
		}

		/*
			Student.
		*/
		static void addStudent(int iDeletionCandidateIndex = -1)
		{
			Dictionary<string, string> fields;

			if (iDeletionCandidateIndex == -1)
			{
				fields = UI.inputFields(new string[]{
					"firstName",
					"lastName",
					"birthYear",
					"groupId"
				});
			}
			else
			{
				Student student = dbms.getObjects<Student>()[iDeletionCandidateIndex];

				int iGroupIndex = dbms.getIndexById<Group>(student.groupId);

				fields = UI.inputFields(new string[]{
					"firstName",
					"lastName",
					"birthYear",
					"groupId"
				}, new string[]{
					student.firstName,
					student.lastName,
					student.birthYear.ToString(),
					iGroupIndex > -1 ? (iGroupIndex + 1).ToString() : null
				});
			}

			/*
				Validate input data.
			*/
			ValidateResult result = Validator.validateStudent(fields, dbms.getObjects<Group>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[validate error]: {result.sMessage}..");
				return;
			}

			if (fields["groupId"].Length > 0)
				fields["groupId"] = dbms.getObjects<Group>()[int.Parse(fields["groupId"]) - 1].id;

			if (iDeletionCandidateIndex != -1)
				dbms.updateObject<Student>(iDeletionCandidateIndex, fields);
			else
				dbms.insertObject<Student>(0, fields);
			
			UI.showStudents(dbms.getObjects<Student>());
		}

		static void deleteStudent(int iIndex)
		{
			dbms.deleteByIndex<Student>(iIndex);
			UI.showStudents(dbms.getObjects<Student>());
		}

		static void editStudent(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Student>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Console.WriteLine($"\n[edit student | i={iIndex + 1}]:");
			addStudent(iIndex);
		}

		static void displayStudent(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Student>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Student student = dbms.getObjects<Student>()[iIndex];

			Group group = new Group();

			int iGroupIndex = dbms.getIndexById<Group>(student.groupId);
			
			if (iGroupIndex > -1)
				group = dbms.getObjects<Group>()[iGroupIndex];

			UI.showStudent(student, iIndex, group, iGroupIndex);
		}

		/*
			Instructor.
		*/
		static void addInstructor(int iDeletionCandidateIndex = -1)
		{
			Dictionary<string, string> fields;

			if (iDeletionCandidateIndex == -1)
			{
				fields = UI.inputFields(new string[]{
					"firstName",
					"lastName",
					"birthYear",
					"expYears"
				});
			}
			else
			{
				Instructor instructor = dbms.getObjects<Instructor>()[iDeletionCandidateIndex];

				fields = UI.inputFields(new string[]{
					"firstName",
					"lastName",
					"birthYear",
					"expYears"
				}, new string[]{
					instructor.firstName,
					instructor.lastName,
					instructor.birthYear.ToString(),
					instructor.expYears.ToString()
				});
			}

			/*
				Validate input data.
			*/
			ValidateResult result = Validator.validateInstructor(fields);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[validate error]: {result.sMessage}..");
				return;
			}

			if (iDeletionCandidateIndex != -1)
				dbms.updateObject<Instructor>(iDeletionCandidateIndex, fields);
			else
				dbms.insertObject<Instructor>(0, fields);

			UI.showInstructors(dbms.getObjects<Instructor>());
		}

		static void deleteInstructor(int iIndex)
		{
			dbms.deleteIdenticalInstructorIdsFromLessons(dbms.getObjects<Instructor>()[iIndex].id);
			dbms.deleteByIndex<Instructor>(iIndex);
			UI.showInstructors(dbms.getObjects<Instructor>());
		}

		static void editInstructor(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Instructor>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Console.WriteLine($"\n[edit instructor | i={iIndex + 1}]:");
			addInstructor(iIndex);
		}

		static void displayInstructor(int iIndex)
		{
			ValidateResult result = Validator.validateIndex(iIndex, dbms.getObjects<Instructor>().Count);
			if (!result.bSuccess)
			{
				Console.WriteLine($"\n[error]: {result.sMessage}..");
				return;
			}

			Instructor instructor = dbms.getObjects<Instructor>()[iIndex];

			UI.showInstructor(instructor, iIndex, dbms.getLessonsByInstructorId(instructor.id));
		}

		static void Main(string[] commands)
		{
			switch (commands.Length)
			{
				case 1:
					if (commands[0] == "-help")
						helpUsage();
					else if (commands[0] == "-report")
						displayReport();
					else if (commands[0] == "-lessons")
						UI.showLessons(dbms.getObjects<Lesson>());
					else if (commands[0] == "-groups")
						UI.showGroups(dbms.getObjects<Group>());
					else if (commands[0] == "-students")
						UI.showStudents(dbms.getObjects<Student>());
					else if (commands[0] == "-instructors")
						UI.showInstructors(dbms.getObjects<Instructor>());
					break;

				case 2:
				{
					int iIndex = Validator.isNumber(commands[1]) ? int.Parse(commands[1]) - 1 : -1;

					if (commands[0] == "-lessons")
					{
						if (commands[1] == "-add")
							addLesson();
						else
							displayLesson(iIndex);
					}
					else if (commands[0] == "-groups")
					{
						if (commands[1] == "-add")
							addGroup();
						else
							displayGroup(iIndex);
					}
					else if (commands[0] == "-students")
					{
						if (commands[1] == "-add")
							addStudent();
						else
							displayStudent(iIndex);
					}
					else if (commands[0] == "-instructors")
					{
						if (commands[1] == "-add")
							addInstructor();
						else
							displayInstructor(iIndex);
					}
					break;
				}

				case 3:
				{
					int iIndex = Validator.isNumber(commands[2]) ? int.Parse(commands[2]) - 1 : -1;

					if (commands[0] == "-lessons")
					{
						if (commands[1] == "-delete")
							deleteLesson(iIndex);
						else if (commands[1] == "-edit")
							editLesson(iIndex);
					}
					else if (commands[0] == "-groups")
					{
						if (commands[1] == "-delete")
							deleteGroup(iIndex);
						else if (commands[1] == "-edit")
							editGroup(iIndex);
					}
					else if (commands[0] == "-students")
					{
						if (commands[1] == "-delete")
							deleteStudent(iIndex);
						else if (commands[1] == "-edit")
							editStudent(iIndex);
					}
					else if (commands[0] == "-instructors")
					{
						if (commands[1] == "-delete")
							deleteInstructor(iIndex);
						else if (commands[1] == "-edit")
							editInstructor(iIndex);
					}
					break;
				}

				default:
					helpUsage();
					break;
			}
		}

		static void helpUsage()
		{
			Console.Write($"\n{System.IO.File.ReadAllText("helpUsage.txt")}\n");
		}
	}
}