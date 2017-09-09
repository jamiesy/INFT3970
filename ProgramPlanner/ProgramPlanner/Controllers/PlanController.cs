﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProgramPlanner.Models;

namespace ProgramPlanner.Controllers
{
    public class PlanController : Controller
    {
        private ProgramPlannerContext db = new ProgramPlannerContext();

        // GET: Plan
        public ActionResult Index()
        {
            Setup.InitializeCourseCode(db);

            ViewBag.UnitsPerDegree = 240;
            ViewBag.SubjectsPerSemester = 4;

            getCourseCodes();

            getDegreeCores(); //doesn't include optional cores yet

            getMajorCores();

            getDegreeOptionalSlots();

            //getDirectedSlots();

            //pass in list of major cores
            //pass in list of directed courses
            //pass in all the optional things


            //ViewBag.Directeds = new SelectList(db.Courses, "CourseID", "CourseCode");

            //YearDegree yearDegree = db.YearDegrees.Find("1");            
            //ViewBag.YearDegreeSelected = "";
            //ViewBag.DegreeCoreCourses = 


            return View(db.StudyAreas.ToList());
        }

        private void getCourseCodes()
        {
            //courses to be searched for in the search box
            List<String> courseCodes = new List<String>();
            foreach (var item in db.Courses)
            {
                courseCodes.Add(item.CourseCode);
            }
            ViewBag.CourseCodeList = courseCodes;
        }

        private void getDegreeCores()
        {
            //pass in list of degree cores based on semester
            int yearDegreeID = 5;   //will be passed in from main menu
            var myYearDegree = db.YearDegrees.Find(yearDegreeID);

            List<String> semester1Cores = new List<String>();
            List<String> semester2Cores = new List<String>();

            foreach (var item in myYearDegree.DegreeCores)
            {
                IQueryable<SemesterCourse> temp = db.SemesterCourses.Where(i => i.CourseID == item.CourseID);
                foreach (var semCourse in temp)
                {
                    //make sure the semester course we are dealing with is the one matching our yeardegree
                    if (semCourse.Year == myYearDegree.Year)
                    {
                        if (semCourse.SemesterID == 1)
                        {
                            semester1Cores.Add(item.Course.CourseCode);
                        }
                        else
                        {
                            semester2Cores.Add(item.Course.CourseCode);
                        }
                    }
                }
            }
            ViewBag.Sem1Cores = semester1Cores;
            ViewBag.Sem2Cores = semester2Cores;
        }

        private void getMajorCores()
        {
            //pass in list of major cores based on semester
            int majorID = 2;   //will be passed in from main menu
            var myMajor = db.Majors.Find(majorID);

            List<String> semester1MajorCores = new List<String>();
            List<String> semester2MajorCores = new List<String>();

            foreach (var item in myMajor.MajorCores)
            {
                IQueryable<SemesterCourse> temp = db.SemesterCourses.Where(i => i.CourseID == item.CourseID);
                foreach (var semCourse in temp)
                {
                    //make sure the semester course we are dealing with is the one matching our major's yeardegree
                    if (semCourse.Year == myMajor.YearDegree.Year)
                    {
                        if (semCourse.SemesterID == 1)
                        {
                            semester1MajorCores.Add(item.Course.CourseCode);
                        }
                        else
                        {
                            semester2MajorCores.Add(item.Course.CourseCode);
                        }
                    }
                }
            }
            ViewBag.Sem1MajorCores = semester1MajorCores;
            ViewBag.Sem2MajorCores = semester2MajorCores;
        }

        private void getDegreeOptionalSlots()
        {
            //pass in list of degree cores based on semester
            int yearDegreeID = 5;   //will be passed in from main menu
            var myYearDegree = db.YearDegrees.Find(yearDegreeID);

            List<string[]> degreeSlots = new List<string[]>();

            foreach (var coreSlot in myYearDegree.DegreeCoreSlots)
            {
                //get all the course codes for this slot and add them to the first element of the string array
                string[] strArr = new string[2];

                Course firstCourse = null; //course we get semester from
                Boolean firstCodeRetrieved = false;

                foreach (var optionalCore in coreSlot.OptionalCoreCourses)
                {
                    strArr[0] += optionalCore.Course.CourseCode + " ";

                    //if code we are getting semester from hasn't been fetched yet, get it now
                    if (!firstCodeRetrieved)
                    {
                        firstCourse = optionalCore.Course;
                        firstCodeRetrieved = true;
                    }
                }

                //get the semester for the first item in the array, we will use that
                IQueryable<SemesterCourse> tempSemCourse = db.SemesterCourses.Where(i => i.CourseID == firstCourse.CourseID);
                foreach (var semCourse in tempSemCourse)
                {
                    //make sure the semester course we are dealing with is the one matching our major's yeardegree
                    if (semCourse.Year == myYearDegree.Year)
                    {
                        strArr[1] = semCourse.SemesterID.ToString();
                    }
                }

                degreeSlots.Add(strArr);

            }

            ViewBag.DegreeSlots = degreeSlots;
        }

        private void getDirectedSlots()
        {
            //pass in list of directeds
            int majorID = 2;   //will be passed in from main menu
            var myMajor = db.Majors.Find(majorID);

            List<String> directedSlots = new List<String>();

            foreach (var directedSlot in myMajor.DirectedSlots)
            {
                string temp = "";
                foreach (var optionalDirected in directedSlot.OptionalDirecteds)
                {
                    temp += optionalDirected.Course.CourseCode + " ";
                }
                directedSlots.Add(temp);
            }

            ViewBag.DegreeSlots = directedSlots;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "InputData")]Plan plan)
        {
            string blah = plan.InputData;
            return View();
        }
    }
}