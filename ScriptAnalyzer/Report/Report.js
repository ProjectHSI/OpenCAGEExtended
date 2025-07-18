function toggleExpandableSection(section) {
	if (!section.classList.contains('active')) {
		section.classList.add('active');
		section.querySelector('.expandableSectionHeader').classList.add('active');
		Array.from(section.getElementsByClassName('.expandableSectionAdditionalClick')).forEach((additionalClickElement) => {
			additionalClickElement.classList.add('active');
		});
		section.querySelector('.expandableSectionContent').classList.add('active');
	} else {
		section.classList.remove('active');
		section.querySelector('.expandableSectionHeader').classList.remove('active');
		Array.from(section.getElementsByClassName('.expandableSectionAdditionalClick')).forEach((additionalClickElement) => {
			additionalClickElement.classList.remove('active');
		});
		section.querySelector('.expandableSectionContent').classList.remove('active');
	}
}

Array.from(document.getElementsByClassName('expandableSection')).forEach(section => {
	section.querySelector('.expandableSectionHeader').addEventListener('click', () => {
		toggleExpandableSection(section);
	});
	Array.from(section.getElementsByClassName('.expandableSectionAdditionalClick')).forEach((additionalClickElement) => {
		additionalClickElement.addEventListener('click', () => {
			toggleExpandableSection(section);
		})
	});
});

function openHash() {
	const hash = window.location.hash.substring(1); //remove preceeding '#'

	if (hash == "") {
		return; // no hash, nothing to do
	}

	const hashElement = document.getElementById(hash);

	if (hashElement == null) {
		console.log("What?", hash);
	}

	// walk and expand everything needed.
	let currentWalkElement = hashElement;
	while (currentWalkElement != null && currentWalkElement.tagName != "BODY") {
		console.log("iteration");

		if (currentWalkElement.classList.contains('expandableSection') && !currentWalkElement.classList.contains('active')) {
			toggleExpandableSection(currentWalkElement);
		}
		currentWalkElement = currentWalkElement.parentElement;
	}

	hashElement.scrollIntoView({ behavior: 'smooth' });

	/*let currentScrollY = window.scrollY

	setTimeout(() => {
		window.scrollTo(0, currentScrollY);
		hashElement.scrollIntoView({ behavior: 'smooth' });
	}, 10)*/
}

window.onpageshow = openHash;
window.onhashchange = openHash;

//console.log("new parser!")