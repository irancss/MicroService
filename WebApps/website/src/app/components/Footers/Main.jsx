"use client";
import Link from "next/link";
import { footerData } from "@/data/siteData/footer";
import { motion } from "framer-motion";

const sectionVariants = {
  hidden: { opacity: 0, y: 30 },
  visible: (i) => ({
    opacity: 1,
    y: 0,
    transition: { delay: i * 0.15, duration: 0.5, type: "spring" },
  }),
};

export default function MainFooter() {
  return (
    <footer className="bg-[#f5f6fa] py-12  shadow-lg">
      <div className="max-w-5xl mx-auto px-4">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {Object.entries(footerData.legalLinks).map(([sectionName, links], i) => (
            <motion.div
              key={sectionName}
              className="footer-section rounded-2xl p-6 bg-[#f8fafc] shadow-neumorph"
              custom={i}
              initial="hidden"
              whileInView="visible"
              viewport={{ once: true, amount: 0.2 }}
              variants={sectionVariants}
            >
              <h4 className="text-base font-semibold text-gray-700 mb-3 tracking-tight">
                {sectionName.replace('section', 'بخش ').replace(/([A-Z])/g, ' $1').trim()}
              </h4>
              <ul className="space-y-1">
                {links.map((link) => (
                  <li key={`${sectionName}-${link.url}`}>
                    <Link
                      href={link.url}
                      className="text-sm text-gray-500 hover:text-blue-600 transition-colors"
                    >
                      {link.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </motion.div>
          ))}
        </div>
      </div>
      <style jsx global>{`
        .shadow-neumorph {
          box-shadow:
            6px 6px 12px #e2e4ea,
            -6px -6px 12px #ffffff;
        }
      `}</style>
    </footer>
  );
}