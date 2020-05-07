﻿using System.Web;
using System.Web.Optimization;

namespace SAAsProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/clean-blog.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/font").Include(
                "~/Content/all.min.css"));

            bundles.Add(new StyleBundle("~/Content/stylecss").Include(
               "~/Content/clean-blog.css"));
        }
    }
}
