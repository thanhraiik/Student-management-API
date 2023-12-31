﻿using BusinessLogicLayer.DTO;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace DataAccessLayer.Interfaces
{
	public interface ITeacherRepository
	{
		Teachers GetTeacher(int id);
		ICollection<Teachers> GetTeachers();
		void AddTeacher(Teachers teacherCreate);
		void UpdateTeacher(int id, TeacherDTO teacherDTO);
		void DeleteTeacher(Teachers teacherDelete);
	}
}
