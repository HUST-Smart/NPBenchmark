﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;


namespace RankPageGenerator {
    public class PageGenerator {
        public const int MaxDateWidth = 19;


        public void generate() {
            using (StreamWriter sw = File.CreateText(CommonCfg.RankPagePath))
            using (HtmlTextWriter htw = new HtmlTextWriter(sw)) {
                htw.WriteLine("<!DOCTYPE html>");
                htw.WriteLine("<html>");
                htw.WriteLine("<head>");
                htw.WriteLine("<meta charset='utf-8' />");
                htw.WriteLine("<title>Benchmark Results</title>");
                htw.WriteLine("<link rel='stylesheet' href='base.css' />");
                htw.WriteLine("</head>");
                htw.WriteLine("<body>");
                htw.WriteLine($"<h1>Instruction</h1><p>{CommonCfg.AuthorInstruction}</p>");
                htw.WriteLine($"<h1>Specification</h1><p>{CommonCfg.SdkSpecification}</p>");
                htw.WriteLine($"<h1>Results</h1>");
                htw.WriteLine("<ol>");
                foreach (var problem in rank.problems) {
                    htw.WriteLine($"<li id='{problem.Key}'><a href='#{problem.Key}'>{problem.Key}</a><ol>");
                    foreach (var instance in problem.Value.instances) {
                        htw.WriteLine($"<li id='{problem.Key}-{instance.Key}'><a href='#{problem.Key}-{instance.Key}'>{instance.Key}</a><table>");
                        htw.WriteLine("<thead><tr><th>Rank</th><th>Author</th><th>Objective</th><th>Date</th><th>Duration</th><th>Algorithm</th><th>Thread</th><th>CPU</th><th>RAM</th><th>Language</th><th>Compiler</th><th>OS</th><th>Email</th><th>Solution</th></tr></thead><tbody>");
                        int count = 0;
                        var results = problem.Value.minimize ? instance.Value.results : instance.Value.results.Reverse();
                        foreach (var result in results) {
                            Submission s = result.header;
                            htw.WriteLine($"<tr><td>{count}</td><td id='auth'>{s.author}</td><td>{s.obj}</td><td>{s.date.Substring(0, Math.Min(s.date.Length, MaxDateWidth))}</td><td>{s.duration}</td><td id='alg'>{s.algorithm}</td><td>{s.thread}</td><td>{s.cpu}</td><td>{s.ram}</td><td>{s.language}</td><td>{s.compiler}</td><td>{s.os}</td><td>{s.email}</td><td><a href='{result.path}'>download<a></td></tr>");
                            ++count;
                        }
                        htw.WriteLine("</tbody></table></li>");
                    }
                    htw.WriteLine("</ol></li>");
                }
                htw.WriteLine("</ol>");
                htw.WriteLine("</body>");
                htw.WriteLine("</html>");
            }
            
            Util.run("git", "add " + CommonCfg.RankPath);
            Util.run("git", "add " + CommonCfg.RankPagePath);
            //Util.run("git", "add " + CommonCfg.RankCssPath);
            Util.run("git", "commit -m a");
            Util.run("git", "push origin gh-pages");
        }


        public Rank rank = Util.Json.load<Rank>(CommonCfg.RankPath);
        public Dictionary<string, Checker.Check> checkers = new Dictionary<string, Checker.Check> {
            { "Luck", Checker.luck.check }
        };
    }
}
